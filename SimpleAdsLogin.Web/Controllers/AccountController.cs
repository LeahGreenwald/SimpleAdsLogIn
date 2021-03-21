using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsLogin.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SimpleAdsLogin.Web.Models;

namespace SimpleAdsLogin.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdsLogin;Integrated Security=true;";
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            SimpleAdsDb db = new SimpleAdsDb(_connectionString);
            db.AddUser(user, password);
            return Redirect("/");
        }
        public IActionResult LogIn()
        {
            LogInViewModel vm = new LogInViewModel();
            if (TempData["errorMessage"] != null)
            {
                vm.ErrorMessage = (string)TempData["errorMessage"];
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            SimpleAdsDb db = new SimpleAdsDb(_connectionString);
            var user = db.LogIn(email, password);
            if (user == null)
            {
                TempData["errorMessage"] = "Invalid email and password combo";
                return Redirect("/account/login");
            }
            var claims = new List<Claim>
            {
                new Claim ("user", email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/newad");
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
        public IActionResult MyAccount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/account/logIn");
            }
            SimpleAdsDb db = new SimpleAdsDb(_connectionString);
            var email = User.Identity.Name;
            var CurrentUser = db.GetByEmail(email);
            MyAccountViewModel vm = new MyAccountViewModel
            {
                Ads = db.MyAccount(CurrentUser.Id)
            };
            return View(vm);
        }
    }
}
