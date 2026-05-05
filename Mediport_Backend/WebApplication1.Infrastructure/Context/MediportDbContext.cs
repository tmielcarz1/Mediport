using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Infrastructure.Context
{
    public class MediportDbContext : DbContext
    {
        public MediportDbContext(DbContextOptions<MediportDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();
        }


    }
}
