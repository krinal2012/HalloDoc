using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace HalloDoc.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(string Email, string Passwordhash)
        {
            NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Database=HelloDoc;User Id=postgres;Password=krinalshah2012;Include Error Detail=True");
            string Query = "select * from \"AspNetUser\"  inner join \"AspNetUserRoles\"  on \"AspNetUser\".\"Id\" = \"AspNetUserRoles\".\"UserId\" inner join \"AspNetRoles\" on \"AspNetUserRoles\".\"RoleId \" = \"AspNetRoles\".\"Id\" where \"Email\"=@Email and \"PasswordHash \"=@PasswordHash";
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@Passwordhash", Passwordhash);
            NpgsqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            int numRows = dataTable.Rows.Count;
            if (numRows > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    HttpContext.Session.SetString("UserName", row["UserName"].ToString());
                    HttpContext.Session.SetString("UserID", row["Id"].ToString());
                    HttpContext.Session.SetString("RoleId", row["Name"].ToString());
                }
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../AdminHome/Index");
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Error()
        {
            return View("../AdminHome/Error");
        }
    }
}
