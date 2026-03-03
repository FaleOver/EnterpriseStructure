using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IStructureNodeRepo StructureNodes { get; }
        public IEmployeeRepo Employees { get; }

        public UnitOfWork(IDbContextFactory<AppDbContext> contextFactory)
        {
            _context = contextFactory.CreateDbContext();

            StructureNodes = new StructureNodeRepo(_context);
            Employees = new EmployeeRepo(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
