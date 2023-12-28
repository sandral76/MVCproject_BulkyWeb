using Microsoft.EntityFrameworkCore;
using MVCproject.Models;

namespace MVCproject.Data
{
    public class BulkyWebDbContext : DbContext
    {
        public BulkyWebDbContext(DbContextOptions<BulkyWebDbContext> options) : base(options) //passing configuration to dbcontext class
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );
        }

    }
}
