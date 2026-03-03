namespace Common.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; init; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
