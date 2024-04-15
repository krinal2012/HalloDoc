using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace HellodocMVC.Controllers
{
    public class RequestByPatientController : Controller
    {
        private readonly HelloDocContext _context;
        private readonly IPatientDash _PatientDash;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RequestByPatientController(HelloDocContext context, IPatientDash PatientDash, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _PatientDash = PatientDash;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult SubmitForMe()
        {
            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            var ViewPatientCreateRequest = _PatientDash.viewMeData(id);
            return View(ViewPatientCreateRequest);
        }
        [HttpPost]
        public IActionResult SubmitForMe(viewPatientReq viewPatientReq)
        {
            _PatientDash.meRequset(viewPatientReq);
            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult SubmitForSomeone()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitForSomeone(viewFamilyReq viewFamilyReq)
        {
            _PatientDash.elseRequset(viewFamilyReq);
            return RedirectToAction("Index", "Dashboard");
        }       
    }
}
