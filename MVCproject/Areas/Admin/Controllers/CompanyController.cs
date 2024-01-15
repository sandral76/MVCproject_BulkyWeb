using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCproject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        public CompanyController(ICompanyRepository dbContext, ICategoryRepository dbContextCategory)
        {
            _companyRepository = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Company> companies=_companyRepository.GetAll();
            return View(companies);
        }
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //create
                Company company = new Company();
                return View(company);

            }
            else
            {
                //update
                var companyFromDb = _companyRepository.GetFirstOrDefault(u => u.Id == id);
                return View(companyFromDb);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                
                if (obj.Id == 0)
                {
                    _companyRepository.Add(obj);

                }
                else
                {
                    _companyRepository.Update(obj);
                    
                }
                _companyRepository.Save();
                TempData["success"] = "Company created successfully"; //obavestenje
                return RedirectToAction("Index");  //nakon dodavanja prikayecemo celu listu cat
            }
            else
            {
                return View(obj);
            }

        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetALl()
        {
           List<Company> objCompanyList = _companyRepository.GetAll().ToList();
           return Json(new { data = objCompanyList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDelted = _companyRepository.GetFirstOrDefault(u => u.Id == id);
            if (companyToBeDelted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });

            }

            _companyRepository.Remove(companyToBeDelted);
            _companyRepository.Save();
            return Json(new { success = true, message = "Deleted Successful" });
        }
        #endregion
    }
}
