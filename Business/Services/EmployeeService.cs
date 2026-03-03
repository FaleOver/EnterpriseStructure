using Business.Abstract;
using Common;
using Common.DTOs;
using DataAccess.Abstract;
using DataAccess.Entities;
using DataAccess.Repositories;
using System.Printing;

namespace Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeDto> CreateAsync(string lastName, string firstName, string middleName)
        {
            ValidateName(firstName, middleName, lastName);

            var employee = new Employee
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
            };
            _unitOfWork.Employees.Add(employee);
            await _unitOfWork.SaveChangesAsync();

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
            };
        }

        public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();

            //if (employees.Count == 0)
            //    throw new BusinessValidationException("Сотрудники не найдены");

            var dtos = employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName,
            }).ToList();

            return dtos;
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id) ??
                throw new BusinessValidationException("Ветка не найдена");

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
            };
        }

        public async Task<EmployeeDto> UpdateAsync(int employeeId, string lastName, string firstName,
            string middleName)
        {
            ValidateName(firstName, middleName, lastName);

            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId) ??
                throw new BusinessValidationException("Сотрудник не найден");

            employee.FirstName = firstName; employee.MiddleName = middleName;
            employee.LastName = lastName;
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
            };
        }

        public async Task<IReadOnlyList<EmployeeDto>> SearchAsync(string? firstName,
            string? middleName, string? lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(middleName)
                && string.IsNullOrWhiteSpace(lastName))
                return await GetAllAsync();

            var employees = await _unitOfWork.Employees.SearchAsync(firstName, middleName, lastName);

            var dtos = employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName,
            }).ToList();

            return dtos;
        }

        private void ValidateName(string firstName, string middleName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new BusinessValidationException("Отсутствует имя");
            if (string.IsNullOrWhiteSpace(middleName))
                throw new BusinessValidationException("Отсутствует фамилия");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new BusinessValidationException("Отсутствует отчество");
        }
    }
}
