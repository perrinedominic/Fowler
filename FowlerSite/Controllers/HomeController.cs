using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FowlerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FowlerSite.Controllers
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
        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Store()
        {
            return View("~/Views/Store/Store.cshtml");
        }

        public IActionResult StoreCart()
        {
            return View("~/Views/Store/StoreCart.cshtml");
        }

        public IActionResult StoreCheckout()
        {
            return View("~/Views/Store/StoreCheckout.cshtml");
        }

        public IActionResult StoreProduct()
        {
            return View("~/Views/Store/StoreProduct.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
