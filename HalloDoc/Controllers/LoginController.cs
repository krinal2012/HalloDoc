using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace HellodocMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly HelloDocContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJWTInterface _jWTInterface;
        public LoginController(HelloDocContext context, IHttpContextAccessor httpContextAccessor, IJWTInterface jWTInterface)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jWTInterface = jWTInterface;
        }
        
        public IActionResult Index()
        {
            return View("../Home/Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = await _context.AspNetUsers.Include(x => x.AspNetUserRoles).FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == Password);
            if (user == null)
            {
                ViewData["Error"] = " Your Username or password is incorrect. ";
                return View("../Home/Login");
            }
            else
            {
                //var jwtToken = _jWTInterface.GenerateJWTAuthetication(user);
                //Response.Cookies.Append("jwt", jwtToken);
                int id = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id).UserId;
                string userName = _context.Users.Where(x => x.AspNetUserId == user.Id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                _httpContextAccessor.HttpContext.Session.SetInt32("id", id);
                _httpContextAccessor.HttpContext.Session.SetString("Name", userName);
                return RedirectToAction("Index", "Dashboard");
            }
           
        }
         public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
