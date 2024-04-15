using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HellodocMVC.Controllers
{
    public class PatientFormController : Controller
    {
        private readonly HelloDocContext _context;
        private readonly IPatientRequest _patientRequest;
        public PatientFormController(HelloDocContext context, IPatientRequest patientRequest)
        {
            _context = context;
            _patientRequest = patientRequest;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PatientReq()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PatientReq(viewPatientReq viewPatientReq)
        {
            _patientRequest.PatientReq(viewPatientReq);
            return View("../PatientForm/Index");
        }
        public IActionResult FamilyReq()
        {
            return View();
        }
        [HttpPost]
        public IActionResult FamilyReq(viewFamilyReq viewFamilyReq  )
        {
            _patientRequest.FamilyReq(viewFamilyReq);
            return View("../PatientForm/Index");
        }
        public IActionResult ConciergeReq()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ConciergeReq(viewConciergeReq viewConciergeReq)
        {
            _patientRequest.ConciergeReq(viewConciergeReq);
            return View("../PatientForm/Index");
        }
        public IActionResult BusinessReq()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BusinessReq(viewBusinessReq viewBusinessReq)
        {
            _patientRequest.BusinessReq(viewBusinessReq);
            return View("../PatientForm/Index");
        }
        [HttpPost]
        public async Task<IActionResult> CheckEmail(string email)
        {
            string message;
            var aspnetuser = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
            if (aspnetuser == null)
            {
                message = "Failure";
            }
            else
            {
                message = "success";
            }
            return Json(new
            {
                Message = message
            });
        }

    }
    
}
