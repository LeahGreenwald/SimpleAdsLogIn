using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsLogin.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsLogin.Data;
using Microsoft.AspNetCore.Authorization;

namespace SimpleAdsLogin.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdsLogin;Integrated Security=true;";
        public IActionResult Index()
        {
            SimpleAdsDb db = new SimpleAdsDb(_connectionString);
            HomeViewModel vm = new HomeViewModel
            {
                Ads = db.GetAllAds(),
                IsAuthenticated = User.Identity.IsAuthenticated
            };
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                vm.CurrentUser = db.GetByEmail(email);
            }
            return View(vm);
        }
        public IActionResult NewAd()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/account/login");
            }
            else
            {
                var email = User.Identity.Name;
                var db = new SimpleAdsDb(_connectionString);
                NewAdViewModel vm = new NewAdViewModel
                {
                    User = db.GetByEmail(email)
                };
                return View(vm);
            }
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var db = new SimpleAdsDb(_connectionString);
            db.AddAd(ad);
            return Redirect("/");
        }
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            SimpleAdsDb db = new SimpleAdsDb(_connectionString);
            db.DeleteAd(id);
            return Redirect("/");
        }
       

    }
}
