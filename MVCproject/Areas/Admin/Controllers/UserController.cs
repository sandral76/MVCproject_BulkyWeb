using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MVCproject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly BulkyWebDbContext _dbContext;
        public UserController(IApplicationUserRepository applicationUserRepository,BulkyWebDbContext dbContext)
        {
            _applicationUserRepository = applicationUserRepository;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetALl()
        {
            List<ApplicationUser> objUserList = _applicationUserRepository.GetAll(includeProperties: "Company").ToList();

            var userRoles = _dbContext.UserRoles.ToList();
            var roles = _dbContext.Roles.ToList();
            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                        { Name = "" };
                }
            }
            return Json(new { data = objUserList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _applicationUserRepository.GetFirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = true, message = "Deleted Successful" });
            }
            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now){
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd=DateTime.Now.AddYears(1000);
            }
            _applicationUserRepository.Save();
            return Json(new { success = true, message = "Operation Successful" });

        }
        
        public IActionResult RoleManager(string id)
        {
            var roleId = _dbContext.UserRoles.FirstOrDefault(u =>u.UserId==id).RoleId;
            UserVM roleVM = new UserVM()
            {
                ApplicationUser = _dbContext.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == id),
                RoleManagers = _dbContext.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Companies = _dbContext.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            roleVM.ApplicationUser.Role = _dbContext.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            return View(roleVM);
        }
        #endregion
    }
}
