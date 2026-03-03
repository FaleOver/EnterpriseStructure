using DataAccess.Abstract;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class EmployeeRepo : IEmployeeRepo
    {
        private readonly DbSet<Employee> _employees;

        public EmployeeRepo(AppDbContext context)
        {
            _employees = context.Employees;
        }

        public async Task AddAsync(Employee employee) =>
            await _employees.AddAsync(employee);

        public async Task<IReadOnlyList<Employee>> GetAllAsync() => 
             await _employees.ToListAsync();

        public async Task<Employee?> GetByIdAsync(int id) =>
             await _employees.FirstOrDefaultAsync(e => e.Id == id);

        public void Update(Employee employee) =>
            _employees.Update(employee);

        public void Delete(Employee employee) =>
            _employees.Remove(employee);

        public async Task<IReadOnlyList<Employee>> SearchAsync(string? firstName, 
            string? middleName, string? lastName)
        {
            IQueryable<Employee> query = _employees;

            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(e => EF.Functions.Like(e.LastName, $"%{lastName.Trim()}%"));
            if (!string.IsNullOrWhiteSpace(firstName))
                query = query.Where(e => EF.Functions.Like(e.FirstName, $"%{firstName.Trim()}%"));
            if (!string.IsNullOrWhiteSpace(middleName))
                query = query.Where(e => EF.Functions.Like(e.MiddleName, $"%{middleName.Trim()}%"));

            return await query.ToListAsync();
        }
    }
}
