using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Infrastructure.Context
{
    public class MediportDbContextFactory : IDesignTimeDbContextFactory<MediportDbContext>
    {
        public MediportDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MediportDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MediportDbContext(optionsBuilder.Options);
        }
    }
}
