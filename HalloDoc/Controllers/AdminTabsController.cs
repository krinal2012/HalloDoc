using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Models;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Controllers
{
    public class AdminTabsController : Controller
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private readonly IAdminTabs _IAdminTabs;
        private readonly IAdminDash _IAdminDash;
        private readonly HelloDocContext _context;
        private readonly INotyfService _notyf;
        public AdminTabsController(IAdminDash IAdminDash, HelloDocContext context, INotyfService notyf, 
            IHttpContextAccessor httpContextAccessor, IAdminTabs IAdminTabs)
        {
            _IAdminDash = IAdminDash;
            _context = context;
            _notyf = notyf;
            _httpContextAccessor = httpContextAccessor;
            _IAdminTabs = IAdminTabs;
        }
        public IActionResult AdminProfile()
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
            var UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            var result = _IAdminTabs.ViewAdminProfile(UserId);
            return View(result);
        }
        public IActionResult ProfilePassword(string Password)
        {
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
            var UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            if (_IAdminTabs.EditPassword(Password, Convert.ToInt32(UserId)))
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }
            return RedirectToAction("AdminProfile");
        }
        [HttpPost]
        public IActionResult EditAdministratorInfo(AdminProfile AdminProfile)
        {
            if (_IAdminTabs.EditAdministratorInfo(AdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("AdminProfile");
        }
        [HttpPost]
        public IActionResult EditBillingInfo(AdminProfile AdminProfile)
        {
            if ( _IAdminTabs.EditBillingInfo(AdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("AdminProfile");
        }

        public IActionResult ProviderMenu()
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var res = _IAdminTabs.PhysicianAll();
            return View(res);
        }
    }
}
