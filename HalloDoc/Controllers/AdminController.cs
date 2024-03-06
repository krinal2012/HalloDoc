using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDash _IAdminDash;
        private readonly HelloDocContext _context;
      
        public AdminController(IAdminDash IAdminDash, HelloDocContext context)
        {
            _IAdminDash = IAdminDash;
            _context = context;
        }
        [CheckAdminAccess]
        public IActionResult Index()
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.CaseReason = _IAdminDash.CaseReason();
            CountStatusWiseRequestModel count = _IAdminDash.CountRequestData();
            return View(count);            
        }

        public IActionResult GetPartialView(string btnName, int statusid, string searchValue)
        {
            var partialview = "_" + btnName;
            var result = _IAdminDash.NewRequestData(statusid, searchValue);
            return PartialView(partialview, result);
        }
        public IActionResult _new()
        {
            var result = _IAdminDash.NewRequestData(1, null);
            return PartialView(result);
          
        }
        public IActionResult viewCase(int RequestId, int RequestTypeId)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var result = _IAdminDash.ViewCaseData(RequestId, RequestTypeId);
            return View(result);    
        }
        [HttpPost]
        public IActionResult viewCase(int RequestId, int RequestTypeId, ViewCaseModel vp)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var result = _IAdminDash.EditViewCaseData(RequestId, RequestTypeId, vp);
            return View(result);
        }
        public IActionResult viewNotes()
        {
            return View();
        }
       
        public IActionResult PhysicianbyRegion(int Regionid)
        {
            var v = _IAdminDash.ProviderbyRegion(Regionid);
            return Json(v);
        }
        [HttpPost]
        public IActionResult AssignCase(int RequestId, int PhysicianId, string Notes)
        {
            _IAdminDash.AssignCaseInfo(RequestId, PhysicianId, Notes);
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult CancleCase(int? RequestId, string Notes, string CaseTag)
        {
            var result = _IAdminDash.CancleCaseInfo(RequestId, Notes, CaseTag);
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult BlockCase(int RequestId, string Notes)
        {
            var res= _IAdminDash.BlockCaseInfo(RequestId, Notes);
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult ViewUploads(int requestid)
        {
            var v = _IAdminDash.ViewUploadsInfo( requestid);
            return View("../Admin/ViewUploads", v);
          
        }
        [HttpPost]
        public IActionResult ViewUploads(viewDocument vp, int userid,IFormFile UploadFile)
        {
            var v = _IAdminDash.ViewUploadPost(vp, userid, UploadFile);
            return ViewUploads(vp.RequestId);
        }
        public IActionResult DeleteFile(int id, int requestid)
        {
            _IAdminDash.DeleteFile(id);
            return RedirectToAction("ViewUploads", new { requestid = requestid });
        }



    }

}


