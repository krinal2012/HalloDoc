using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class AgreementPageController : Controller
    {
        private readonly IAdminDash _IAdminDash;
        private readonly INotyfService _notyf;
        public AgreementPageController(IAdminDash actionrepo, INotyfService notyf)
        {
            _IAdminDash = actionrepo;
            _notyf = notyf;
        }
        public IActionResult Index(int RequestID)
        {
            TempData["RequestID"] = " " + RequestID;
            TempData["PatientName"] = "krinal";
            return View();
        }
        public IActionResult Accept(int RequestID)
        {
            _IAdminDash.SendAgreement_accept(RequestID);
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Reject(int RequestID, string Notes)
        {
            _IAdminDash.SendAgreement_Reject(RequestID, Notes);
            return RedirectToAction("Index", "Admin");
        }
    }
}
