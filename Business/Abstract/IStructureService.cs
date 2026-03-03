using Common.DTOs;
using Common.Enums;

namespace Business.Abstract
{
    public interface IStructureService
    {
        Task<StructureNodeDto> CreateAsync(string name, NodeType nodeType, int parentId);
        Task<IReadOnlyList<StructureNodeDto>> GetAllAsync();
        Task<StructureNodeDto?> GetByIdAsync(int id);
        Task Rename(int structureNodeId, string newName);
        Task DeleteAsync(int id);
    }
}
