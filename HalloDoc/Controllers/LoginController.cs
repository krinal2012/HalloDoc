using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HellodocMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly HelloDocContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJWTInterface _jWTInterface;
        private readonly ILogin _Login;
        public LoginController(HelloDocContext context, IHttpContextAccessor httpContextAccessor, IJWTInterface jWTInterface, ILogin login)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jWTInterface = jWTInterface;
            _Login = login;
        }

        public IActionResult Index()
        {
            return View("../Home/Login");
        }
        public string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = new AspNetUser();
            user.Email = Email;
            user.PasswordHash = Password;
            UserInfo u = await _Login.CheckAccessLogin(user);
            if (user == null)
            {
                ViewData["Error"] = " Your Username or password is incorrect. ";
                return View("../Home/Login");
            }
            else
            {
                var jwtToken = _jWTInterface.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwtToken);
                //int id = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id).UserId;
                //string userName = _context.Users.Where(x => x.AspNetUserId == user.Id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                //_httpContextAccessor.HttpContext.Session.SetInt32("id", id);
                //_httpContextAccessor.HttpContext.Session.SetString("Name", userName);
                return RedirectToAction("Index", "Dashboard");
            }
           
        }
        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear();
        //    return RedirectToAction("Index");
        //}
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "Home");
        }
    }
}
