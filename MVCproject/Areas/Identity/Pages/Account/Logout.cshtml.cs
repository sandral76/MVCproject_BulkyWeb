// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MVCproject.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger,
            IShoppingCartRepository shoppingCartRepository, IApplicationUserRepository applicationUserRepository)
        {
            _signInManager = signInManager;
            _logger = logger;
            _shoppingCartRepository = shoppingCartRepository;   
            _applicationUserRepository = applicationUserRepository;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

     
            List<ShoppingCart> shoppingCarts = _shoppingCartRepository.GetAll(u => u.ApplicationUserId == userID, includeProperties: "Product").ToList();
            _shoppingCartRepository.RemoveRange(shoppingCarts);
            _shoppingCartRepository.Save();


            //ili
            //HttpContext.Session.Clear(); da se izbrise korpa kad se korinik izloguje
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
