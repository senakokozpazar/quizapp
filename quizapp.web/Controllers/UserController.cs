using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using quizapp.data.Models;
using Microsoft.AspNetCore.Http;

namespace quizapp.web.Controllers
{
    public class UserController : Controller
    {
        private readonly QuizappContext _context;

        public UserController(QuizappContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.UserName ?? "Anonim");

                return RedirectToAction("Index", "Quiz");
            }

            ViewBag.Error = "Email veya şifre yanlış";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string userName, string email, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                ViewBag.Error = "Bu email ile daha önce kayıt olunmuş.";
                return View();
            }

            var newUser = new User
            {
                UserName = userName,
                Email = email,
                Password = password
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
