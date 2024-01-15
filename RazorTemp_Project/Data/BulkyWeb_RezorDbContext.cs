using Microsoft.EntityFrameworkCore;
using RazorTemp_Project.Models;

namespace RazorTemp_Project.Data
{
    public class BulkyWeb_RezorDbContext : DbContext
    {
        public BulkyWeb_RezorDbContext(DbContextOptions<BulkyWeb_RezorDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 });
        }

    }

}
