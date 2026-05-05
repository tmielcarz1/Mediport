using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.Interfaces;
using WebApplication1.Infrastructure.Context;
using WebApplication1.Infrastructure.Initializer;
using WebApplication1.Infrastructure.Repositories;

namespace WebApplication1.WebApi.Extensions
{
    /// <summary>
    /// czas życia dla repozytoriów itd.
    /// </summary>
    public static class RepositoriesExtension
    {

        /// <summary>
        /// repozytoria
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<MediportDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            services.AddScoped<IDatabaseInitalizer, DatabaseInitializer>();
            services.AddScoped<ITagsRepository, TagsRepository>();


        }

        /// <summary>
        /// migracja
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static async Task MigrateDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MediportDbContext>();
                await db.Database.MigrateAsync();
            }
        }

        /// <summary>
        /// inicjalizacja
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static async Task InitializeDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitalizer>();
            await initializer.SeedData(app.Lifetime.ApplicationStopping);
        }


    }
}
