using Common.DTOs;

namespace Business.Abstract
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> CreateAsync(string lastName, string firstName, string middleName);
        Task<IReadOnlyList<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(int id);
        Task<EmployeeDto> UpdateAsync(int employeeId, string lastName, string firstName, 
            string middleName);
        Task<IReadOnlyList<EmployeeDto>> SearchAsync(string? firstName,
            string? middleName, string? lastName);
    }
}
