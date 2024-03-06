using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
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
        public LoginController(HelloDocContext context, IHttpContextAccessor httpContextAccessor)

        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public IActionResult Index()
        {
            return View("../Home/Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == Password);
            if (user == null)
            {
                ViewData["Error"] = " Your Username or password is incorrect. ";
                return View("../Home/Login");
            }
            else
            {
                int id =  _context.Users.FirstOrDefault( u => u.AspNetUserId == user.Id).UserId;
                string userName = _context.Users.Where(x => x.AspNetUserId == user.Id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                _httpContextAccessor.HttpContext.Session.SetInt32("id", id);
                _httpContextAccessor.HttpContext.Session.SetString("Name", userName);
                return RedirectToAction("Index", "Dashboard");
            }
        //NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Database=HelloDoc;User Id=postgres;Password=krinalshah2012;Include Error Detail=True");
        //string Query = "SELECT * FROM \"AspNetUser\" where \"Email\"=@Email and \"PasswordHash \"=@PasswordHash";
        //connection.Open();
        //NpgsqlCommand command = new NpgsqlCommand(Query, connection);
        //command.Parameters.AddWithValue("@Email", Email);
        //command.Parameters.AddWithValue("@PasswordHash", PasswordHash);
        //NpgsqlDataReader reader = command.ExecuteReader();
        //DataTable dataTable = new DataTable();  
        //dataTable.Load(reader);
        //int numRows = dataTable.Rows.Count;
        //if (numRows > 0)
        //{
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        HttpContext.Session.SetString("UserName", row["username"].ToString());
        //        HttpContext.Session.SetString("UserID", row["Id"].ToString());
        //    }
        //    return RedirectToAction("Index", "Dashboard");
        //}
        //else
        //{
        //    ViewData["Error"] = " Your Username or password is incorrect. ";
        //    return View("../Home/Login");
        //}
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
