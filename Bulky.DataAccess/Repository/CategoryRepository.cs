using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
namespace Bulky.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private BulkyWebDbContext _dbContext;
        public CategoryRepository(BulkyWebDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
