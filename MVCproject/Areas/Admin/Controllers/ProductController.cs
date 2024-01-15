using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace MVCproject.Areas.Admin.Controllers
{
    //oznaka za area-u kojoj pripada ctrl
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository dbContext, ICategoryRepository dbContextCategory, IWebHostEnvironment webostEnvironment)
        {
            _productRepository = dbContext;
            _categoryRepository = dbContextCategory;
            _webHostEnvironment = webostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _productRepository.GetAll(includeProperties: "Category").ToList();

            return View(objProductList);
        }
        

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _categoryRepository.GetAll().ToList().Select  //preuzimanje vrednosti za category polje
                (u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            //ViewBag.CategoryList = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);

            }
            else
            {
                //update
                productVM.Product = _productRepository.GetFirstOrDefault(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)) //upload new image
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"images\product\" + fileName; //update image url
                }
                if (obj.Product.Id == 0)
                {
                    _productRepository.Add(obj.Product);

                }
                else
                {
                    _productRepository.Update(obj.Product);

                }
                _productRepository.Save();
                TempData["success"] = "Product created successfully"; //obavestenje
                return RedirectToAction("Index");  //nakon dodavanja prikayecemo celu listu cat
            }
            else
            {
                obj.CategoryList = _categoryRepository.GetAll().ToList().Select  //preuzimanje vrednosti za category polje
                (u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
            return View();
        }
        /*public IActionResult Edit(int? id)
        {
            //if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? categoryFromDb = _productRepository.GetFirstOrDefault(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            //za prosledjeni id, obelezja ce biti promenjena
            if (ModelState.IsValid)
            {
                _productRepository.Update(obj);
                _productRepository.Save();
                TempData["success"] = "Product updated successfully"; //obavestenje
                return RedirectToAction("Index");  //nakon dodavanja prikayecemo celu listu cat
            }
            return View();
        }*/
        /*public IActionResult Delete(int? id)
        {
            //if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _productRepository.GetFirstOrDefault(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }*/
        /*[HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            //Product? obj = _productRepository.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _productRepository.Remove(obj);
            _productRepository.Save();
            TempData["success"] = "Product successfully"; //obavestenje
            return RedirectToAction("Index");
        }*/

        #region API CALLS
        [HttpGet]
        public IActionResult GetALl()
        {
            List<Product> objProductList = _productRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDelted  = _productRepository.GetFirstOrDefault(u => u.Id == id);
            if(productToBeDelted == null)
            {
                return Json(new { success = false,message="Error while deleting" });

            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDelted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _productRepository.Remove(productToBeDelted);
            _productRepository.Save();
            return Json(new {success=true,message="Deleted Successful"});
        }
        #endregion
    }
}


