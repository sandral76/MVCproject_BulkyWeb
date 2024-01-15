using Bulky.DataAccess.Data;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BulkyWebDbContext _dbContext;
        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, BulkyWebDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }
        public void Initialize()
        {
            //1.kreiranje migracija ako nisu primenjene

            try
            {
                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            //2.kreiranje uloga ako nisu kreirane
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())  //proverice da li uloge postoje prvi put, ako ne onda ce kreirati sve samo prvi put
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                //3.ako uloge nisu kreirane, kreiracemo admina
                    _userManager
                .CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin Admin",
                    PhoneNumber = "1111117",
                    StreetAddress = "test 123 Ave",
                    State = "Il",
                    PostalCode = "23564",
                    City = "Test",
                }, "Admin123#").GetAwaiter().GetResult();
            ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
            _userManager.AddToRoleAsync(user,SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
            
            
        }
    }
}
