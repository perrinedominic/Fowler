using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FowlerSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models;
using Microsoft.Data.SqlClient;
using FowlerSite.Services;
using Microsoft.Extensions.Configuration;

namespace FowlerSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the key/value application configuration properties.
        /// </summary>
        public IConfiguration Configuration { get; }

        public IActionResult Index()
        {
            var gameList = new ListService(Configuration).GetGames();
            var imageList = new BlobService(Configuration.GetConnectionString("AzureStorage")).ListBlobsAsync();

            return View(gameList);
        }

        public IActionResult ContactUs()
        {
            return View();
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
