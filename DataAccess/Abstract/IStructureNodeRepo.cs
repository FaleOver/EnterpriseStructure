using DataAccess.Entities;

namespace DataAccess.Abstract
{
    public interface IStructureNodeRepo
    {
        Task Add(StructureNode node);
        Task<IReadOnlyList<StructureNode>> GetAllAsync();
        Task<StructureNode?> GetByIdAsync(int id);
        Task<IReadOnlyList<StructureNode>> GetByEmployeeIdAsync(int id);
        Task Update(StructureNode node);
        Task DeleteAsync(StructureNode node);
    }
}
