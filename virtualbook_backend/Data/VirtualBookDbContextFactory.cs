using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace virtualbook_backend.Data
{
    public class VirtualBookDbContextFactory : IDesignTimeDbContextFactory<VirtualBookDbContext>
    {
        public VirtualBookDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Create DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<VirtualBookDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new VirtualBookDbContext(optionsBuilder.Options);
        }
    }
}
