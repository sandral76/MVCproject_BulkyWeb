using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace MVCproject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, 
            ICategoryRepository categoryRepository,IShoppingCartRepository shoppingCartRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _shoppingCartRepository=shoppingCartRepository;
        }

        public IActionResult Index()
        {
           IEnumerable<Product> products = _productRepository.GetAll();
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _productRepository.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId

            };
            return View(cart);

        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId= userID;
            ShoppingCart cartFromDb= _shoppingCartRepository.GetFirstOrDefault(u=>u.ApplicationUserId==userID && u.ProductId==shoppingCart.ProductId);
            if (cartFromDb != null)
            {
                //scart exists
                cartFromDb.Count += shoppingCart.Count;
                _shoppingCartRepository.Update(cartFromDb);
                _shoppingCartRepository.Save();

            }
            else
            {
                _shoppingCartRepository.Add(shoppingCart);
                _shoppingCartRepository.Save();
                //kad god dodajemo u korpu dodajemo tu vrednost u sesiju
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _shoppingCartRepository.GetAll(u => u.ApplicationUserId == userID).Count());
            }

            TempData["success"] = "Cart updated successufully";
            return RedirectToAction(nameof(Index)); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
