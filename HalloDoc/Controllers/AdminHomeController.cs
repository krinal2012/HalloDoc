using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class AdminHomeController : Controller
    {
        private readonly ILogin _Login;
        private readonly IJWTInterface _jwtinterface;
        public AdminHomeController(ILogin loginRepository,IJWTInterface jwtService)
        {
            _Login = loginRepository;
            _jwtinterface = jwtService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(AspNetUser aspNetUser)
        {
            UserInfo u = await _Login.CheckAccessLogin(aspNetUser);

            if (u != null)
            {
                var jwttoken = _jwtinterface.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                Response.Cookies.Append("Status", "1");
                Response.Cookies.Append("StatusName", "new");
                if (u.Role == "Patient")
                {
                    return RedirectToAction("Index", "DashBoard");
                }
                else if (u.Role == "Physician")
                {
                    return Redirect("~/Physician/DashBoard");
                    //return RedirectToAction("Index", "Admin");
                }
                return Redirect("~/Admin/DashBoard");
                //return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../AdminHome/Login");
            }
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "AdminHome");
        }  
        public IActionResult Error()
        {
            return View("../AdminHome/Error");
        }
    }
}
