using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hallodoc.Controllers
{
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
        public IActionResult Index()
        {
           int id= (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            var result = _PatientDash.PatientList(id);
            //var UserIDForRequest = _context.Users.Where(r => r.AspNetUserId == sessionCV.UserID()).FirstOrDefault();

            //if (UserIDForRequest != null)
            //{
            //    List<DataModels.Request> Request = _context.Requests.Where(r => r.UserId == UserIDForRequest.UserId).ToList();
            //    List<int> ids = new List<int>();

            //    foreach (var request in Request)
            //    {
            //        var doc = _context.RequestWiseFiles.Where(r => r.RequestId == request.RequestId).FirstOrDefault();
            //        if (doc != null)
            //        {
            //            ids.Add(request.RequestId);
            //        }
            //    }
            //    ViewBag.docidlist = ids;
            //    ViewBag.listofrequest = Request;
            //}
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
