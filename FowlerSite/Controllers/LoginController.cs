using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FowlerSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace FowlerSite.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View("Login");
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}