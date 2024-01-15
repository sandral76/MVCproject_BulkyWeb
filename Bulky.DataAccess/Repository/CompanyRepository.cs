using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly BulkyWebDbContext _dbContext;
        public CompanyRepository(BulkyWebDbContext dbContext) : base(dbContext)
        {
            _dbContext=dbContext;
        }

 
        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(Company company)
        {
            _dbContext.Update(company);
        }
    }
}
