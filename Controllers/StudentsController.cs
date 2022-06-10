  using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CloudDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudDemo.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Index()
        {
            var ID = HttpContext.Session.GetString("ID");
            if (string.IsNullOrEmpty(ID))
                return RedirectToAction("Index", "Login");
            var role = HttpContext.Session.GetString("Role");
            if (role.ToLower() == "student")
            {
                HttpClient client = new HttpClient();
                var response = client.GetAsync($"http://158.101.230.122:8080/exams/examineId/{ID}").Result;
                if (response != null)
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
        public IActionResult Solve(string ExamID)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"http://158.101.230.122:8080/exams/questions/examID/{ExamID}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            List<QuestionModel> questions = JsonSerializer.Deserialize<List<QuestionModel>>(content);
            SolveViewModel model = new SolveViewModel();
            model.questions = questions;
            model.ExamID = ExamID;
            return View("Solve", model);
        }

        [HttpPost]
        public IActionResult Solve(List<StudentAnswerModel> Answers)
        {
            HttpClient client = new HttpClient();
            foreach(var Answer in Answers)
            {
                var SerializedAnswer= JsonSerializer.Serialize(Answer);
                var SeriliazedModel = new StringContent(SerializedAnswer, Encoding.UTF8, "application/json");
                client.PostAsync("http://158.101.230.122:8080/answers", SeriliazedModel);
            }
            return RedirectToAction("Index");
        }
    }
}
