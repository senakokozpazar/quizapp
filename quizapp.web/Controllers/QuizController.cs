using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quizapp.data.Models;
using Microsoft.AspNetCore.Http;

namespace quizapp.web.Controllers
{
    public class QuizController : Controller
    {
        private readonly QuizappContext _context;

        public QuizController(QuizappContext context)
        {
            _context = context;
        }

        // GET: /Quiz/
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

            // Geri tuşu için önceki soru id’si
            var previousQuestion = _context.Questions
                .Where(q => q.QuizId == quizId && q.QuestionId < question.QuestionId)
                .OrderByDescending(q => q.QuestionId)
                .FirstOrDefault();

            if (previousQuestion != null)
            {
                ViewBag.HasPreviousQuestion = true;
                ViewBag.PreviousQuestionId = previousQuestion.QuestionId;
            }
            else
            {
                ViewBag.HasPreviousQuestion = false;
            }

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

            int currentScore = HttpContext.Session.GetInt32("Score") ?? 0;
            if (isCorrect) currentScore++;
            HttpContext.Session.SetInt32("Score", currentScore);

            TempData["IsCorrect"] = isCorrect;
            TempData["SelectedAnswerId"] = selectedAnswerId;

            var nextQuestion = _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuizId == quizId && q.QuestionId > questionId)
                .OrderBy(q => q.QuestionId)
                .FirstOrDefault();

            if (nextQuestion == null)
            {
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
