using DataAccess.Abstract;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace DataAccess.Repositories
{
    public class StructureNodeRepo : IStructureNodeRepo
    {
        private readonly AppDbContext _context;

        public StructureNodeRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(StructureNode node)
        {
            await _context.StructureNodes.AddAsync(node);
        }

        public async Task<IReadOnlyList<StructureNode>> GetAllAsync()
        {
            return await _context.StructureNodes.ToListAsync();
        }

        public async Task<StructureNode?> GetByIdAsync(int id)
        {
            return await _context.StructureNodes.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IReadOnlyList<StructureNode>> GetByEmployeeIdAsync(int id)
        {
            return await _context.StructureNodes.Where(n => n.EmployeeId == id).ToListAsync();
        }

        public async Task Update(StructureNode node)
        {
            _context.StructureNodes.Update(node);
        }
            
        public async Task DeleteAsync(StructureNode node)
        {
            // Обобщенное табличное выражение (CTE), которое рекурсивно находит 
            // сам узел и всех его потомков на любой глубине, а затем удаляет их
            var sql = @"
                WITH NodeTree AS (
                    SELECT Id FROM StructureNodes WHERE Id = {0}
                    UNION ALL
                    SELECT n.Id FROM StructureNodes n
                    INNER JOIN NodeTree t ON n.ParentId = t.Id
                )
                DELETE FROM StructureNodes WHERE Id IN (SELECT Id FROM NodeTree)";

            await _context.Database.ExecuteSqlRawAsync(sql, node.Id);
        }
    }
}
