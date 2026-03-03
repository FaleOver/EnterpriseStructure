using Common.Enums;

namespace Common.DTOs
{
    public class StructureNodeDto
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public NodeType NodeType { get; init; }

        public int? ParentId { get; init; }
        public List<StructureNodeDto> Children { get; set; } = new();

        public int? EmployeeId { get; set; }
        public EmployeeDto? Employee { get; set; }
        public DateOnly? HireDate { get; set; }
    }
}
