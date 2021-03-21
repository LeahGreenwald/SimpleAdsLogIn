using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsLogin.Data;

namespace SimpleAdsLogin.Web.Models
{
    public class HomeViewModel
    {
        public List<Ad> Ads { get; set; }
        public User CurrentUser { get; set; }
        public bool IsAuthenticated { get; set; }
    }
    public class NewAdViewModel
    {
        public User User { get; set; }
    }
    public class LogInViewModel
    {
        public string ErrorMessage { get; set; }
    }
    public class MyAccountViewModel
    {
        public List<Ad> Ads { get; set; }
    }
}
