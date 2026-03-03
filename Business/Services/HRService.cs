using Business.Abstract;
using Common;
using Common.Enums;
using DataAccess.Abstract;

namespace Business.Services
{
    public class HRService : IHRService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HRService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HireEmployeeAsync(int positionId, int employeeId, DateOnly hireDate)
        {
            var position = await _unitOfWork.StructureNodes.GetByIdAsync(positionId) ??
                throw new BusinessValidationException("Позиция не найдена");
            if (position.NodeType != NodeType.Position)
                throw new BusinessValidationException("Сотрудника можно назначить только на должность");
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId) ??
                throw new BusinessValidationException("Сотрудник не найден");

            position.EmployeeId = employeeId;
            position.HireDate = hireDate;

            _unitOfWork.StructureNodes.Update(position);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DismissEmployeeAsync(int positionId)
        {
            var position = await _unitOfWork.StructureNodes.GetByIdAsync(positionId) ??
                throw new BusinessValidationException("Позиция не найдена");
            if (position.NodeType != NodeType.Position)
                throw new BusinessValidationException("Сотрудника можно снять только с должности");

            position.EmployeeId = null;
            position.HireDate = null;

            _unitOfWork.StructureNodes.Update(position);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId) ??
                throw new BusinessValidationException("Сотрудник не найден");

            var employeeNodes = await _unitOfWork.StructureNodes.GetByEmployeeIdAsync(employeeId);

            foreach (var node in employeeNodes)
            {
                node.EmployeeId = null;
                node.HireDate = null;
                _unitOfWork.StructureNodes.Update(node);
            }

            _unitOfWork.Employees.Delete(employee);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
