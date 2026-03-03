using DataAccess.Abstract;
using Common.Enums;

namespace DataAccess.Entities
{
    public class StructureNode : BaseEntity
    {
        public string Name { get; set; } = null!;
        public NodeType NodeType { get; set; }

        public int? ParentId { get; set; }
        public StructureNode? Parent { get; set; }
        public ICollection<StructureNode> Children { get; set; } = new List<StructureNode>();

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateOnly? HireDate { get; set; }
    }
}
