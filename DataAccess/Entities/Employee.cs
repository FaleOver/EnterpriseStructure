using DataAccess.Abstract;

namespace DataAccess.Entities
{
    public class Employee : BaseEntity
    {
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
    }
}
