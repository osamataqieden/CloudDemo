using CloudDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CloudDemo.Controllers
{
    public class ExamController : Controller
    {
        public IActionResult Index()
        {
            var ID = HttpContext.Session.GetString("ID");
            if (string.IsNullOrEmpty(ID))
                return RedirectToAction("Index", "Login");
            var role = HttpContext.Session.GetString("Role");
            if (role.ToLower() == "admin")
            {
                HttpClient client = new HttpClient();
                var response = client.GetAsync($"http://158.101.230.122:8080/exams/examinerId/{ID}").Result;
                if(response != null)
                {
                    var StringResponse = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(StringResponse))
                    {
                        var model = JsonSerializer.Deserialize<List<ExamModel>>(StringResponse);
                        return View(model);
                    }
                }
                List<ExamModel> list = new List<ExamModel>();
                return View(list);
            }
            return RedirectToAction("Index", "Students");
        }

        public IActionResult Generate()
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync("http://158.101.231.162:8080/api/rest/iam/user/getByRole/Student").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            ViewBag.Users = new SelectList(JsonSerializer.Deserialize<List<UsersViewModel>>(content), "externalId", "firstName");

            return View();
        }

        [HttpPost]
        public IActionResult Generate(NewExamModel model)
        {
            HttpClient client = new HttpClient();
            model.examinerId = HttpContext.Session.GetString("ID");
            model.examineId = model.externalId;
            var SeriliazedModel = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://158.101.230.122:8080/exams/generateByType", SeriliazedModel).Result.Content;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ShowQuestions(string examid)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"http://158.101.230.122:8080/exams/questions/examID/{examid}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            List<QuestionModel> questions = JsonSerializer.Deserialize<List<QuestionModel>>(content);
            return View("Questions",questions);
        }
        
    }
}
