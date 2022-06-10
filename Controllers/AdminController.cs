using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudDemo.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var ID = HttpContext.Session.GetString("ID");
            if(string.IsNullOrEmpty(ID))
                return RedirectToAction("Index", "Login");
            var role = HttpContext.Session.GetString("Role");
            if(role.ToLower() == "admin")
            {
                return View();
            }
            return RedirectToAction("Index", "Students");
        }
    }
}
