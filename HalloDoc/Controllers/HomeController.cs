using Microsoft.AspNetCore.Mvc;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace HellodocMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogin  _Ilogin;
        private readonly HelloDocContext _context;
        private readonly INotyfService _notyf;
        private static IHttpContextAccessor _httpContextAccessor;
        public HomeController(ILogin Ilogin, HelloDocContext context, INotyfService notyf, IHttpContextAccessor httpContextAccessor)
        {
            _Ilogin = Ilogin;
            _context = context;
            _notyf = notyf;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult ForgotPass()
        {
            return View();
        }
        public IActionResult ResetEmail(string Email)
        {
            if (_Ilogin.SendResetLink(Email))
            {
                //_notyf.Success("Mail Send  Successfully..!");
            }
            else
            {
                _notyf.Error("User not exist.");
            }
            return RedirectToAction("ForgotPass", "Home");
        }
        [HttpGet]
        public IActionResult ResetPass(string email, string datetime)
        {
           TempData["email"] = email;
            //TimeSpan time = DateTime.Now - DateTime.Parse(datetime);
            //if (time.TotalHours > 24)
            //{
            //    return View("LinkExpired");
            //}
            //else
            //{
                return View();
            //}
        }
        [HttpPost]
        public IActionResult SavePassword(viewPatientReq viewPatientReq)
        {
            var aspnetuser = _context.AspNetUsers.FirstOrDefault(m => m.Email == viewPatientReq.Email);
            if (aspnetuser != null)
            {
                aspnetuser.PasswordHash = viewPatientReq.Pass;
                _context.AspNetUsers.Update(aspnetuser);
                _context.SaveChangesAsync();
                TempData["emailmessage"] = "Your password is changed!!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                TempData["emailmessage"] = "Email is not registered!!";
                return View("ResetPass");
            }
        }
        public IActionResult Register(string Email)
        {
            viewPatientReq res = new viewPatientReq();
            res.Email = Email;
            return View(res);
        }
        public IActionResult CreateAccount(viewPatientReq viewPatientReq)
        {
            var res = _Ilogin.CreateAccount(viewPatientReq);
            if (res)
            {
                _notyf.Success("Account Created successfully");
                return View("Login");
            }
            else
            {
                _notyf.Error("You are already registered...");
                return View("Register");
            }
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}