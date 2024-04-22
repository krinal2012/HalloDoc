using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Models;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using static HalloDoc.Repository.Repository.JWTService;

namespace HalloDoc.Controllers
{

    [CustomAuthorize("Patient")]
    public class DashboardController : Controller
    {
        private readonly HelloDocContext _context;
        private readonly IPatientDash _PatientDash;
        private readonly IAdminTabs _IAdminTabs;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardController(HelloDocContext context, IPatientDash PatientDash, IHttpContextAccessor httpContextAccessor, IAdminTabs iAdminTabs)
        {
            _context = context;
            _PatientDash = PatientDash;
            _httpContextAccessor = httpContextAccessor;
            _IAdminTabs = iAdminTabs;
        }
        public IActionResult Index(string sortColumn, string sortOrder, int pagesize = 5, int page = 1)
        {
            // int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            int id = Int32.Parse(Crredntials.UserID());
            var result = _PatientDash.PatientList(id, page, pagesize, sortColumn, sortOrder);
            return View(result);

        }
        public IActionResult UploadDocument(int RequestId)
        {
            var result = _PatientDash.viewDocuments(RequestId);
            return View(result);
        }
        [HttpPost]
        public IActionResult UploadDocument(int RequestId, IFormFile? UploadFile)
        {
            _PatientDash.uploadDocument(RequestId, UploadFile);
            return RedirectToAction("UploadDocument", new { RequestId = RequestId });
        }
        public IActionResult PatientProfile()
        {
            //int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            int id = Int32.Parse(Crredntials.UserID());
            var result = _PatientDash.viewProfile(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult PatientProfile(viewProfile vp)
        {
           // int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            int id = Int32.Parse(Crredntials.UserID());
            _PatientDash.EditProfile(id, vp);
            return View();
        }
        public IActionResult EditRole(string roleid)
        {
            int Roleid = Int32.Parse(roleid);
            var result = _IAdminTabs.ViewEditRole(Roleid);
            return View(result);
        }
    }
}
