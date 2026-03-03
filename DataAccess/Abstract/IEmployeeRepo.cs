using DataAccess.Entities;

namespace DataAccess.Abstract
{
    public interface IEmployeeRepo
    {
        Task AddAsync(Employee entity);
        Task<IReadOnlyList<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        void Update(Employee entity);
        void Delete(Employee entity);
        Task<IReadOnlyList<Employee>> SearchAsync(string? firstName, 
            string? middleName, string? lsatName);
    }
}
