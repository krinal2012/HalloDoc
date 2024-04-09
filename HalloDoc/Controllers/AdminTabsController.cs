using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Models;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Drawing;
using static HalloDoc.Entity.Models.Constant;

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
        public IActionResult ProviderLocation()
        {
            ViewBag.Log = _IAdminTabs.FindPhysicianLocation();
            return View();
        }
        public IActionResult AdminProfile(string UserId)
        {
            if(UserId == null)
            {
                var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
                UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            }
            ViewData["Heading"] = "My Profile";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
           var result = _IAdminTabs.ViewAdminProfile(UserId);
            return View(result);
        }
        public IActionResult AddAdminProfile()
        {
            ViewData["Heading"] = "Add Admin Account";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            return View("AdminProfile");
        }
        public IActionResult AddAdminAccount(AdminProfile admindata, int[] checkboxes)
        {
           bool res = _IAdminTabs.AddAdminAccount(admindata, checkboxes);
            return RedirectToAction("AdminProfile");
        }
        
        public IActionResult ProfilePassword(string Password)
        {
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
            var UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            if (_IAdminTabs.ProfilePassword(Password, Convert.ToInt32(UserId)))
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
        public IActionResult ProviderMenu(int region=-1)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var res = _IAdminTabs.PhysicianAll(region);
            return View(res);
        }
        public IActionResult changeNoti(int[] files, int region=-1)
        {
            //bool res = _IAdminTabs.changeNoti(files);
            if (_IAdminTabs.changeNoti(files, region))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("ProviderMenu");
        }
        [HttpPost]
        public IActionResult ContactProviderMail(string Email, string Message)
        {
            bool res = _IAdminTabs.ContactProviderMail(Email, Message);
            return RedirectToAction("ProviderMenu");
        }
        public IActionResult EditProvider(int PhysicianId)
        {
            ViewData["Heading"] = "Edit";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.Role = _IAdminTabs.RolePhyscian();
            var result = _IAdminTabs.ViewProviderProfile(PhysicianId);
            return View(result);
        }
        public IActionResult AddProvider()
        {
            ViewData["Heading"] = "Add";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.Role = _IAdminTabs.RolePhyscian();
            return View("EditProvider");
        }
        public IActionResult EditPassword(int PhysicianId,string Password)
        {
            if (_IAdminTabs.EditPassword(Password, PhysicianId))
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }
            return RedirectToAction("EditProvider", new { PhysicianId= PhysicianId });
        }
        public IActionResult EditAdministrator(PhysiciansData physiciansData)
        {
            if (_IAdminTabs.EditAdministrator(physiciansData))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditProvider", new { PhysicianId = physiciansData.Physicianid });
        }
        public IActionResult EditBilling(PhysiciansData physiciansData)
        {
            if (_IAdminTabs.EditBilling(physiciansData))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditProvider", new { PhysicianId = physiciansData.Physicianid });
        }
        public IActionResult EditProviderProfile(PhysiciansData physiciansData)
        {
            if (_IAdminTabs.EditProviderProfile(physiciansData))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditProvider", new { PhysicianId = physiciansData.Physicianid });
        }
        public IActionResult SaveProvider(int[] checkboxes, int physicianid)
        {
            bool res = _IAdminTabs.SaveProvider(checkboxes, physicianid);
            return RedirectToAction("ProviderMenu");
        }
        public IActionResult AddAccount(PhysiciansData physicianData, int[] checkboxes)
        {
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
            var UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "AspNetUserId").Value;
            bool res = _IAdminTabs.AddProviderAccount(physicianData, checkboxes, UserId);
            return RedirectToAction("ProviderMenu");
        }
        public IActionResult DeleteProvider(int PhysicianId)
        {
            bool res = _IAdminTabs.DeleteProvider(PhysicianId);
            _notyf.Success("Account Deleted..");
            return RedirectToAction("Index", "Provider");
        }
        public IActionResult AccessAccount()
        {
            var res = _context.Roles.Where(r=> r.IsDeleted == new BitArray(1)). ToList();
            return View(res);
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        public IActionResult EditRole(int RoleId)
        {
            var result = _IAdminTabs.ViewEditRole(RoleId);
            return View(result);
        }
        public IActionResult RolebyAccountType(AccountType Account)
        {
            var v = _IAdminTabs.RolebyAccountType(Account);
            return Json(v);
        }
        public IActionResult SaveCreateRole(CreateRole roles)
        {
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
            var UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "AspNetUserId").Value;
            var v = _IAdminTabs.SaveCreateRole(roles,UserId);
            _notyf.Success("Role Created Successfully");
            ModelState.Clear();
            return View("CreateRole");
        }
        public IActionResult SaveEditRole(CreateRole roles)
        {
            var v = _IAdminTabs.SaveEditRole(roles);
            _notyf.Success("Role Edited Successfully");
            return RedirectToAction("EditRole" , new { RoleId = roles.RoleId});
        }
        public IActionResult DeleteRole(int RoleId)
        {
            bool res = _IAdminTabs.DeleteRole(RoleId);
            if(res==true)
            {
                _notyf.Success("Role Deleted..");
            }
            return RedirectToAction("AccessAccount");
        }
        public IActionResult AccessUser(string AccountType)
        {
            ViewBag.AspNetRole = _IAdminDash.AspNetRole();
            var res = _IAdminTabs.UserAccessData(AccountType);
            return View(res);
        }
        public IActionResult Partners(string searchValue, int Profession)
        {
            ViewBag.Professions = _context.HealthProfessionalTypes.ToList();
            var res = _IAdminTabs.PartnersData(searchValue, Profession);
            return View(res);
        }
        public IActionResult PartnersAddEdit(int VendorId)
        {
            if (VendorId == 0)
            {
                ViewData["Heading"] = "Add";
            }
            else
            {
                ViewData["Heading"] = "Update";
            }
            ViewBag.Professions = _context.HealthProfessionalTypes.ToList();
            var result = _IAdminTabs.EditPartners(VendorId);
            return View("PartnersAddEdit",result);
        }
        public IActionResult EditPartnersData(HealthProfessional hp)
        {
            var result = _IAdminTabs.EditPartnersData(hp);
            if (result==true)
            {
                _notyf.Success("Data edited Successfully...");
            }
            else
            {
                _notyf.Error("Data not Changed...");
            }
            return RedirectToAction("Partners");
        }
        public IActionResult DeleteBusiness(int VendorId)
        {
            bool res = _IAdminTabs.DeleteBusiness(VendorId);
            if (res == true)
            {
                _notyf.Success("Business Deleted Successfully..");
            }
            return RedirectToAction("Partners");
        }
        public IActionResult RecordsPatient(string firstname, string lastname, string email, string phone)
        {
            var res = _IAdminTabs.PatientHistory(firstname, lastname, email, phone);
            return View("RecordsPatient", res);
        }
        public IActionResult RecordsPatientExplore(int UserId)
        {
            var res= _IAdminTabs.RecordsPatientExplore(UserId);
            return View(res);
        }
    }
}
