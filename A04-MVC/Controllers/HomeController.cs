/*
* FILE         : HomeController.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Negin Karimi
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Controller for user login and logout operations
*/

using System.Diagnostics;
using A04_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace A04_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
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
