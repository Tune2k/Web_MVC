using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-EFUPT8E\\TUNE;Database=MyAsp;Trusted_Connection=True;TrustServerCertificate=True;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
