using Microsoft.AspNetCore.Mvc;
using MVCproject.Data;
using MVCproject.Models;

namespace MVCproject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BulkyWebDbContext _dbContext;
        public CategoryController(BulkyWebDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _dbContext.Categories.ToList();
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
                _dbContext.Categories.Add(obj);
                _dbContext.SaveChanges();
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
            Category? categoryFromDb = _dbContext.Categories.Find(id);
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
                _dbContext.Categories.Update(obj);
                _dbContext.SaveChanges();
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
            Category? categoryFromDb = _dbContext.Categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _dbContext.Categories.Find(id);
            if(obj== null)
            {
                return NotFound();
            }
            _dbContext.Categories.Remove(obj);
            _dbContext.SaveChanges();
            TempData["success"] = "Category deleted successfully"; //obavestenje
            return RedirectToAction("Index");
        }
    }
}

