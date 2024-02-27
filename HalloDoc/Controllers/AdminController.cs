using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDash _IAdminDash;

        public AdminController(IAdminDash IAdminDash)
        {
            _IAdminDash = IAdminDash;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetPartialView(string btnName, int statusid)
        {
            var partialview = "_" + btnName;
            var result = _IAdminDash.NewRequestData(statusid);
            return PartialView(partialview, result);
        }
        public IActionResult _new(int statusid)
        {
            var result = _IAdminDash.NewRequestData(statusid);
            return PartialView( result);
          
        }
        public IActionResult viewCase(int RequestId, int RequestTypeId)
        {
            var result = _IAdminDash.ViewCaseData(RequestId, RequestTypeId);
            return View(result);
        }
        public IActionResult viewNotes()
        {
           
            return View();
        }
       
    }
}
