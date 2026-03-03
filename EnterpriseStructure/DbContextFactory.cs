using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Presentation
{
    public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=MyWpfAppDb;Trusted_Connection=True;MultipleActiveResultSets=true",
                b => b.MigrationsAssembly("Presentation"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}