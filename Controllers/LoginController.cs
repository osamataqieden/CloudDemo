using CloudDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudDemo.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("ID") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                if(role != null)
                {
                    if (role.ToLower() == "admin")
                        return RedirectToAction("Index", "Admin");
                    else return RedirectToAction("Index", "Students");
                }
            }
            ViewBag.InvalidUserNameOrPassword = "N";
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginModel loginModel)
        {
            HttpClient client = new HttpClient();
            var entity = JsonSerializer.Serialize(loginModel);
            var requestContent = new StringContent(entity, Encoding.UTF8, "application/json");
            var response =  client.PostAsync("http://158.101.231.162:8080/api/rest/iam/login", requestContent).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var responseModel = JsonSerializer.Deserialize<LoginResponseModel>(content);
            if ((int)response.StatusCode == 201 || (int)response.StatusCode == 200)
            {
                HttpContext.Session.SetString("ID", responseModel.externalId);
                HttpContext.Session.SetString("Role", responseModel.role);
                if (responseModel.role == "Student")
                {
                    return RedirectToAction("Index", "Students");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            else
            {
                ViewBag.InvalidUserNameOrPassword = "Y";
                return View();
            }
        }
    }
}
