using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

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
