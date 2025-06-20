using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quizapp.data.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace quizapp.web.Controllers
{
    public class QuizController : Controller
    {

        private readonly QuizappContext _context;

        public QuizController(QuizappContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var list = _context.Quizzes.ToList();
            return View(list);
        }

        public IActionResult Start(int quizId, int questionId = 0)
        {
            Question question;

            if (questionId == 0)
            {
                question = _context.Questions
                    .Include(q => q.Answers)
                    .Where(q => q.QuizId == quizId)
                    .OrderBy(q => q.QuestionId)
                    .FirstOrDefault();

                // Skoru sıfırla, yeni quiz başlatılıyor
                HttpContext.Session.SetInt32("Score", 0);
            }
            else
            {
                question = _context.Questions
                    .Include(q => q.Answers)
                    .FirstOrDefault(q => q.QuestionId == questionId && q.QuizId == quizId);
            }

            if (question == null)
            {
                return NotFound("Soru bulunamadı.");
            }

            ViewBag.QuizId = quizId;

            // Doğru/yanlış sonucunu göster (SubmitAnswer’dan geliyor)
            if (TempData.ContainsKey("IsCorrect"))
                ViewBag.IsCorrect = (bool)TempData["IsCorrect"];

            if (TempData.ContainsKey("SelectedAnswerId"))
                ViewBag.SelectedAnswerId = (int)TempData["SelectedAnswerId"];

            return View(question);
        }

        [HttpPost]
        public IActionResult SubmitAnswer(int quizId, int questionId, int selectedAnswerId)
        {
            var selectedAnswer = _context.Answers.FirstOrDefault(a => a.AnswerId == selectedAnswerId);

            if (selectedAnswer == null)
                return BadRequest("Geçersiz cevap.");

            bool isCorrect = selectedAnswer.IsCorrect ?? false;

            // Skoru sessionda tut
            int currentScore = HttpContext.Session.GetInt32("Score") ?? 0;
            if (isCorrect) currentScore++;
            HttpContext.Session.SetInt32("Score", currentScore);

            // TempData ile sonucu Start view'a aktar
            TempData["IsCorrect"] = isCorrect;
            TempData["SelectedAnswerId"] = selectedAnswerId;

            // Sonraki soruyu al
            var nextQuestion = _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuizId == quizId && q.QuestionId > questionId)
                .OrderBy(q => q.QuestionId)
                .FirstOrDefault();

            if (nextQuestion == null)
            {
                // Quiz bitti, skoru gönder
                return RedirectToAction("QuizFinished");
            }

            return RedirectToAction("Start", new { quizId = quizId, questionId = nextQuestion.QuestionId });
        }

        public IActionResult QuizFinished()
        {
            int finalScore = HttpContext.Session.GetInt32("Score") ?? 0;
            ViewBag.FinalScore = finalScore;
            return View();
        }
    }
}
