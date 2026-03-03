using Common.Enums;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<StructureNode> StructureNodes { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public AppDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StructureNode>(entity =>
            {
                entity.ToTable("StructureNodes");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.NodeType)
                    .IsRequired();
                entity.HasIndex(n => new { n.ParentId, n.Name })
                    .IsUnique();
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasData(
                    new StructureNode()
                    {
                        Id = 1,
                        Name = "Заводоуправление",
                        NodeType = NodeType.Department,
                        ParentId = null,
                    });
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.MiddleName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
