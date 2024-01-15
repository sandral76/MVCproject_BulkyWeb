using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace MVCproject.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    [BindProperties]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailsRepository;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository,
            IApplicationUserRepository applicationUserRepository,
            IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailsRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _applicationUserRepository = applicationUserRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailsRepository = orderDetailsRepository;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCarts = _shoppingCartRepository.GetAll(u => u.ApplicationUserId == userID, includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                double price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (price * cart.Count);
            }
            return View(ShoppingCartVM);

        }
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _shoppingCartRepository.GetFirstOrDefault(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _shoppingCartRepository.Update(cartFromDb);
            _shoppingCartRepository.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _shoppingCartRepository.GetFirstOrDefault(u => u.Id == cartId, tracked: true);
            if (cartFromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _shoppingCartRepository.GetAll(u => u.ApplictionUser == cartFromDb.ApplictionUser).Count() - 1);

                _shoppingCartRepository.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _shoppingCartRepository.Update(cartFromDb);

            }
            _shoppingCartRepository.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _shoppingCartRepository.GetFirstOrDefault(u => u.Id == cartId,tracked:true);
            HttpContext.Session.SetInt32(SD.SessionCart, _shoppingCartRepository.GetAll(u => u.ApplictionUser == cartFromDb.ApplictionUser).Count()-1);

            _shoppingCartRepository.Remove(cartFromDb);

            _shoppingCartRepository.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCarts = _shoppingCartRepository.GetAll(u => u.ApplicationUserId == userID, includeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _applicationUserRepository.GetFirstOrDefault(u => u.Id == userID);
            //u summary delu mozemo da izmenimo podatke za dostavu npr 
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                double price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (price * cart.Count);
            }
            return View(ShoppingCartVM);

        }
        //metoda kad se ide na order confirm nakon order details
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCarts = _shoppingCartRepository.GetAll(u => u.ApplicationUserId == userID, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUser = _applicationUserRepository.GetFirstOrDefault(u => u.Id == userID);
            //u summary delu mozemo da izmenimo podatke za dostavu npr 
            ApplicationUser applicationUser = _applicationUserRepository.GetFirstOrDefault(u => u.Id == userID);

            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                double price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (price * cart.Count);
            }
            if (ShoppingCartVM.OrderHeader.ApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //regular customer acc, need to capture payment
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _orderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
            _orderHeaderRepository.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCarts)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Product.Price,
                    Count = cart.Count
                };
                _orderDetailsRepository.Add(orderDetail);
                _orderDetailsRepository.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7039/";
                StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/shoppingCart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/shoppingCart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                 };

                foreach (var item in ShoppingCartVM.ShoppingCarts)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();

                Session session = service.Create(options);
                _orderHeaderRepository.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _orderHeaderRepository.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });

        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader=_orderHeaderRepository.GetFirstOrDefault(u=>u.Id==id,includeProperties:"ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service=new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid") { 
                    _orderHeaderRepository.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _orderHeaderRepository.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _orderHeaderRepository.Save();
                }
                HttpContext.Session.Clear();
            }
            List<ShoppingCart> shoppingCarts = _shoppingCartRepository.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _shoppingCartRepository.RemoveRange(shoppingCarts);
            _shoppingCartRepository.Save();
            return View(id);

        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }

            }
        }
    }
}
