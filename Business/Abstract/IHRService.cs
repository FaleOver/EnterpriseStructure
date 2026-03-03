namespace Business.Abstract
{
    public interface IHRService
    {
        Task HireEmployeeAsync(int positionId, int employeeId, DateOnly hireDate);
        Task DismissEmployeeAsync(int positionId);
        Task DeleteEmployeeAsync(int employeeId);
    }
}
