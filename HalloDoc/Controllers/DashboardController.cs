using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hallodoc.Controllers
{
    //[CustomAuthorize("1")]
    public class DashboardController : Controller
    {
        private readonly HelloDocContext _context;
        private readonly IPatientDash _PatientDash;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardController(HelloDocContext context, IPatientDash PatientDash, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _PatientDash = PatientDash;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index(string sortColumn, string sortOrder, int pagesize = 5, int page = 1)
        {       
            if (_httpContextAccessor.HttpContext.Session.GetInt32("id") == null)
            {
                return View("../Home/Login");
            }
            else
            {
                int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
                var result = _PatientDash.PatientList(id, page, pagesize, sortColumn, sortOrder);
                return View(result);
            }
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
            return RedirectToAction("UploadDocument", new { id = RequestId });
        }   
        public IActionResult PatientProfile()
        {
            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            var result=_PatientDash.viewProfile(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult PatientProfile(viewProfile vp)
        {
            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            _PatientDash.EditProfile(id, vp);
            return View();
        }
    }
}
