using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private WallContext dbContext;

        public HomeController(WallContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("curUser") == null) {
                return View();
            }
            else {
                int curUser = (int)HttpContext.Session.GetInt32("curUser");
                User user = dbContext.Users
                .Include(u => u.CreatedMessages)
                .Include(u => u.CreatedComments)
                .FirstOrDefault(u => u.UserId == curUser);
                ViewBag.user = user;
                List<Message> AllMessages = dbContext.Messages
                    .Include(m => m.User)
                    .Include(m => m.Responses)
                    .ThenInclude(c => c.User)
                    .ToList();
                ViewBag.allMessages = AllMessages;
                return View("Wall");
            }
        }

        [HttpGet]
        [Route("/deleteMessage/{messageId}")]
        public IActionResult DeleteMessage(int messageId) {
            if(HttpContext.Session.GetInt32("curUser") == null) {
                return RedirectToAction("Index");
            }
            if(!dbContext.Messages.Any(u => u.MessageId == messageId)) {
                return RedirectToAction("Index");
            }
            Message selectedMessage = dbContext.Messages
                .Include(c => c.User)
                .FirstOrDefault(w => w.MessageId == messageId);
            int curUser = (int)HttpContext.Session.GetInt32("curUser");
            if(selectedMessage.User.UserId != curUser) {
                return RedirectToAction("Index");
            }
            dbContext.Messages.Remove(selectedMessage);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/deleteComment/{commentId}")]
        public IActionResult DeleteComment(int commentId) {
            if(HttpContext.Session.GetInt32("curUser") == null) {
                return RedirectToAction("Index");
            }
            if(!dbContext.Comments.Any(u => u.CommentId == commentId)) {
                return RedirectToAction("Index");
            }
            Comment selectedComment = dbContext.Comments
                .Include(c => c.User)
                .FirstOrDefault(w => w.CommentId == commentId);
            int curUser = (int)HttpContext.Session.GetInt32("curUser");
            if(selectedComment.User.UserId != curUser) {
                return RedirectToAction("Index");
            }
            dbContext.Comments.Remove(selectedComment);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/message/new")]
        public IActionResult CreateMessage(Message newMessage) {
            newMessage.UserId = (int)HttpContext.Session.GetInt32("curUser");
            dbContext.Add(newMessage);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/comment/new/{messageId}")]
        public IActionResult CreateComment(int messageId, Comment newComment) {
            newComment.MessageId = messageId;
            newComment.UserId = (int)HttpContext.Session.GetInt32("curUser");
            dbContext.Add(newComment);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid) {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("curUser", user.UserId);
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult ProcessLogin(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "There is no user with this email address!");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Incorrect Password!");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("curUser", userInDb.UserId);
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
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
