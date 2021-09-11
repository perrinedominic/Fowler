using FowlerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FowlerSite.Controllers
{
    public class StoreController : Controller
    {
        private readonly ILogger<StoreController> _logger;

        public StoreController(ILogger<StoreController> logger)
        {
            _logger = logger;
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

        public IActionResult StoreCatalog()
        {
            return View("~/Views/Store/StoreCatalog.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
