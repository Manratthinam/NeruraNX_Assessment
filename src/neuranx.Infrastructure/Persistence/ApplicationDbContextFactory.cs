using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace neuranx.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory
      : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            // Add the following using directive:
            // using Microsoft.Extensions.Configuration;
            // And ensure you have the Microsoft.Extensions.Configuration.FileExtensions package installed.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(basePath, "../neuranx.Api"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
