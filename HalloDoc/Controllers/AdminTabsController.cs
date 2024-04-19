using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Models;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data;
using System.Web.Helpers;
using Twilio.TwiML.Messaging;
using Twilio.TwiML.Voice;
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
            if (UserId == null)
            {
                var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
                UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            }
            ViewData["Heading"] = "My Profile";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.Role = _IAdminTabs.RoleAdmin();
            var result = _IAdminTabs.ViewAdminProfile(UserId);
            return View(result);
        }
        public IActionResult AddAdminProfile()
        {
            ViewData["Heading"] = "Add Admin Account";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.Role = _IAdminTabs.RoleAdmin();
            return View("AddAdminProfile");
        }
        public IActionResult AddAdminAccount(AdminProfile admindata)
        {
            bool res = _IAdminTabs.AddAdminAccount(admindata);
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
            if (_IAdminTabs.EditBillingInfo(AdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("AdminProfile");
        }
        public IActionResult ProviderMenu(int page, int region = -1)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var res = _IAdminTabs.PhysicianAll(region, page);
            return View(res);
        }
        public IActionResult changeNoti(int[] files, int region = -1)
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
        public IActionResult ContactProviderMail(string Email, string Message, int radio)
        {
            bool result = false;
            bool sms = false;
            if (radio == 1)
            {
                sms = _IAdminDash.SendMessage(Message);
            }
            else if (radio == 2)
            {
                result = _IAdminTabs.ContactProviderMail(Email, Message);
            }
            else
            {
                result = _IAdminTabs.ContactProviderMail(Email, Message);
                sms = _IAdminDash.SendMessage(Message);
            }
            if (result)
            {
                _notyf.Success("Email sent Successfully.");
            }
            if (sms)
            {
                _notyf.Success("Message sent Successfully.");
            }
            return RedirectToAction("ProviderMenu");
        }
        public IActionResult EditProvider(int PhysicianId)
        {
            ViewData["Heading"] = "Edit Physician Account";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.Role = _IAdminTabs.RolePhyscian();
            var result = _IAdminTabs.ViewProviderProfile(PhysicianId);
            return View(result);
        }
        public IActionResult ProviderProfile(int PhysicianId)
        {
            ViewData["Heading"] = "My Profile";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.Role = _IAdminTabs.RolePhyscian();
            var result = _IAdminTabs.ViewProviderProfile(PhysicianId);
            return View("EditProvider", result);
        }
        public IActionResult AddProvider()
        {
            ViewData["Heading"] = "Add";
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.Role = _IAdminTabs.RolePhyscian();
            return View("AddProvider");
        }
        public IActionResult EditPassword(int PhysicianId, string Password)
        {
            if (_IAdminTabs.EditPassword(Password, PhysicianId))
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }
            return RedirectToAction("EditProvider", new { PhysicianId = PhysicianId });
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
        public IActionResult AccessAccount(int page)
        {
            var res = _IAdminTabs.AccessAccount(page);
            return View(res);
        }
        public IActionResult CreateRole()
        {
            return View();
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
            if (roles.files != null)
            {
                var v = _IAdminTabs.SaveCreateRole(roles, UserId);
                _notyf.Success("Role Created Successfully");
            }
            else
            {
                TempData["Errormessage"] = "Please Select one of the role!!";
                return View("CreateRole");
            }
            ModelState.Clear();
            return View("CreateRole");
        }
        public IActionResult SaveEditRole(CreateRole roles)
        {
            var v = _IAdminTabs.SaveEditRole(roles);
            _notyf.Success("Role Edited Successfully");
            return RedirectToAction("EditRole", new { RoleId = roles.RoleId });
        }
        public IActionResult DeleteRole(int RoleId)
        {
            bool res = _IAdminTabs.DeleteRole(RoleId);
            if (res == true)
            {
                _notyf.Success("Role Deleted..");
            }
            return RedirectToAction("AccessAccount");
        }
        public IActionResult AccessUser(string AccountType, int page)
        {
            ViewBag.AspNetRole = _IAdminDash.AspNetRole();
            var res = _IAdminTabs.UserAccessData(AccountType, page);
            return View(res);
        }
        public IActionResult Partners(string searchValue, int Profession, int page)
        {
            ViewBag.Professions = _context.HealthProfessionalTypes.ToList();
            var res = _IAdminTabs.PartnersData(searchValue, Profession, page);
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
            return View("PartnersAddEdit", result);
        }
        public IActionResult EditPartnersData(HealthProfessional hp)
        {
            var result = _IAdminTabs.EditPartnersData(hp);
            if (result == true)
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
        public IActionResult RecordsPatient(SearchInputs search)
        {
            var res = _IAdminTabs.PatientHistory(search);
            return View("RecordsPatient", res);
        }
        public IActionResult RecordsPatientExplore(int UserId)
        {
            var res = _IAdminTabs.RecordsPatientExplore(UserId);
            return View(res);
        }
        public IActionResult RecordsBlock(BlockHistory Formdata)
        {
            var res = _IAdminTabs.RecordsBlock(Formdata);
            return View(res);
        }
        public IActionResult UnBlock(int reqId)
        {
            bool res = _IAdminTabs.UnBlock(reqId);
            return RedirectToAction("RecordsBlock");
        }
        public IActionResult RecordsSearch(SearchInputs search)
        {
            var res = _IAdminTabs.RecordsSearch(search);
            return View(res);
        }
        public IActionResult RecordsDelete(int reqId)
        {
            bool var = _IAdminTabs.RecordsDelete(reqId);
            if (var)
            {
                _notyf.Success("Record deleted successfully");

            }
            return RedirectToAction("RecordsSearch");
        }
        public IActionResult RecordsEmailLog(SearchInputs search)
        {
            var res = _IAdminTabs.RecordsEmailLog(search);
            return View(res);
        }
        public IActionResult RecordsSMSLog(SearchInputs search)
        {
            var res = _IAdminTabs.RecordsSMSLog(search);
            return View(res);
        }
        public IActionResult EditRole(string roleid)
        {
            int Roleid = Int32.Parse(roleid);
            var result = _IAdminTabs.ViewEditRole(Roleid);
            return View(result);
        }
        public IActionResult ContactAdmin(string Notes)
        {
            bool Contact = _IAdminTabs.ContactAdmin(Convert.ToInt32(Crredntials.UserID()), Notes);
            if (Contact)
            {
                _notyf.Success("Mail Send Succesfully");
            }
            else
            {
                _notyf.Error("Mail Not Send Succesfully");
            }
            return RedirectToAction("ProviderProfile", new { PhysicianId = Convert.ToInt32(Crredntials.UserID()) });
        }
       

    }
}
