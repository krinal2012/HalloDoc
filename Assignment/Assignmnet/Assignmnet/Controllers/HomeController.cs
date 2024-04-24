using Assignment.Entity.DataModels;
using Assignment.Entity.Models;
using Assignment.Repository.Repository.Interface;
using Assignmnet.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignmnet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientInfo _patientInfo;
        public HomeController(ILogger<HomeController> logger, IPatientInfo patientInfo)
        {
            _logger = logger;
            _patientInfo = patientInfo;
        }

        public IActionResult Index(PaginatedViewModel<Patient> ls)
        {
            var PatientList = _patientInfo.RequestData(ls);
            return View(PatientList);
        }
        public IActionResult AddPatient(PaginatedViewModel<Patient> ls)
        {
            bool res=  _patientInfo.AddPatient(ls);
            return RedirectToAction("Index");
        }
        public IActionResult editPatientModal(int id)
        {
            var Patientinfo = _patientInfo.editPatientModal(id);
            if(Patientinfo ==null)
            {
                return NotFound();
            }
            return View("../PartialView/_EditPatient", Patientinfo); 
        }
        public IActionResult editPatientPost(Patient formData)
        {
            bool Patientinfo = _patientInfo.editPatientPost(formData);
            return RedirectToAction("Index");

        }
        public IActionResult DeletePatient(int id)
        {
            bool res = _patientInfo.DeletePatient(id);
            return RedirectToAction("Index");
        }

        
    }
}