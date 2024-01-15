using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;


namespace Bulky.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly BulkyWebDbContext _dbContext;
        public ApplicationUserRepository(BulkyWebDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

       
        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(ApplicationUser applicationUser)
        {
           _dbContext.Update(applicationUser);
        }


    }
}
