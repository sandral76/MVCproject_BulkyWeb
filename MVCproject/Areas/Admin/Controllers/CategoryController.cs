using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MVCproject.Areas.Admin.Controllers
{
    //oznaka za area-u kojoj pripada ctrl
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]  //samo korisnici sa ulogom admin mogu da pristupaju
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository dbContext)
        {
            _categoryRepository = dbContext;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _categoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(obj);
                _categoryRepository.Save();
                TempData["success"] = "Category created successfully"; //obavestenje
                return RedirectToAction("Index");  //nakon dodavanja prikayecemo celu listu cat
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _categoryRepository.GetFirstOrDefault(u => u.Id == id);
            //Category categoryFromDb = _dbContext.Categories.FirstOrDefault(u=>u.Id==id); //radi isto sto i prva metoda
            //Category categoryFromDb = _dbContext.Categories.Where(u => u.Id == id).FirstOrDefault(categoryFromDb); //radi isto sto i prva metoda
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            //za prosledjeni id, obelezja ce biti promenjena
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(obj);
                _categoryRepository.Save();
                TempData["success"] = "Category updated successfully"; //obavestenje
                return RedirectToAction("Index");  //nakon dodavanja prikayecemo celu listu cat
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _categoryRepository.GetFirstOrDefault(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _categoryRepository.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _categoryRepository.Remove(obj);
            _categoryRepository.Save();
            TempData["success"] = "Category deleted successfully"; //obavestenje
            return RedirectToAction("Index");
        }
    }
}

