
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using HalloDoc.Entity.DataContext;

namespace HellodocMVC.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly HelloDocContext _context;
       
        public HomeController(HelloDocContext context)
        {
            _context = context;
            
        }
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

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
        //[HttpGet]
        //public IActionResult ResetPass(string email, string datetime)
        //{
        //    //Encyptdecypt en = new Encyptdecypt();
        //    TempData["email"] = email;
        //    TimeSpan time = DateTime.Now - DateTime.Parse(datetime);
        //    if (time.TotalHours > 24)
        //    {
        //        return View("LinkExpired");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public IActionResult SavePassword(string email, string Password)
        //{
        //    //var hasher = new PasswordHasher<string>();
        //    //string hashedPassword = hasher.HashPassword(null, Password);

        //    var aspnetuser = _context.AspNetUsers.FirstOrDefault(m => m.Email == email);
        //    //if (aspnetuser != null)
        //    //{
        //        aspnetuser.PasswordHash = Password;
        //        _context.AspNetUsers.Update(aspnetuser);
        //        _context.SaveChangesAsync();

        //        TempData["emailmessage"] = "Your password is changed!!";
        //        return RedirectToAction("Login", "Home");
        //    //}
        //    //else
        //    //{
        //    //    TempData["emailmessage"] = "Email is not registered!!";
        //    //    return View("ResetPass");
        //    //}
        //}
        //public async Task<IActionResult> ResetEmail(string Email)
        //{
        //    if (await CheckregisterdAsync(Email))
        //    {
        //        /*var aspnetuser = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == Email);
        //        //aspnetuser.PasswordHash = generatepass();
        //        //aspnetuser.ModifiedDate = DateTime.Now;
        //        try
        //        {
        //            _context.Update(aspnetuser);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AspnetuserExists(aspnetuser.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }*/
        //        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        //        string email = Email;
        //        DateTime dateTime = DateTime.Now;
        //        string datetime = dateTime.ToString();
        //        string resetLink = $"https://localhost:7025/Home/ResetPass?email={email}&datetime={datetime}";

        //        //send mail
        //        MailMessage MM = new()
        //        {
        //            //From = new MailAddress(_emailConfig.From),
        //            From = new MailAddress("k@gmail.com"),
        //            Subject = "Change PassWord"
        //        };
        //        MM.To.Add(new MailAddress(Email));
        //        MM.Body = $@"
        //        <html>
        //        <body>
        //            <p>We received a request to reset your password.</p>
        //            <p>To reset your password, click the following link:</p>
        //            <p><a href=""{resetLink}"">Reset Password</a></p>
        //            <p>If you didn't request a password reset, you can ignore this email.</p>
        //        </body>
        //        </html>";
        //        MM.IsBodyHtml = true;
        //        using (var smtpClient = new SmtpClient(_emailConfig.SmtpServer))
        //        {
        //            smtpClient.Port = _emailConfig.Port;
        //            smtpClient.Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password);
        //            smtpClient.EnableSsl = true;

        //            smtpClient.Send(MM);
        //        }
        //        TempData["EmailCheck"] = "Your ID Pass Send In Your Mail";
        //    }
        //    else
        //    {
        //        TempData["EmailCheck"] = "Your Email Is not registered";          
        //    }
        //    return RedirectToAction("ForgotPass", "Home");
        //}
        //private bool AspnetuserExists(string id)
        //{
        //    return (_context.AspNetUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
        //public async Task<bool> CheckregisterdAsync(string email)
        //{
        //    string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        //    if (!string.IsNullOrEmpty(email) && Regex.IsMatch(email, pattern))
        //    {

        //        var U = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
        //        if (U != null)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
        //private static Random random = new Random();
        //public static string generatepass()
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    return new string(Enumerable.Repeat(chars, 8)
        //        .Select(s => s[random.Next(s.Length)]).ToArray());
        //}
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}