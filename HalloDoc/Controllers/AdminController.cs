using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            CountStatusWiseRequestModel count = _IAdminDash.CountRequestData();
            return View(count);
            
        }

        public IActionResult GetPartialView(string btnName, int statusid)
        {
            var partialview = "_" + btnName;
            var result = _IAdminDash.NewRequestData(statusid);
            return PartialView(partialview, result);
        }
        public IActionResult _new()
        {
            var result = _IAdminDash.NewRequestData(1);
            return PartialView(result);
          
        }
        public IActionResult viewCase(int RequestId, int RequestTypeId)
        {
            var result = _IAdminDash.ViewCaseData(RequestId, RequestTypeId);
            return View(result);
        }
        [HttpPost]
        public IActionResult viewCase(int RequestId, int RequestTypeId, ViewCaseModel vp)
        {
            var result = _IAdminDash.EditViewCaseData(RequestId, RequestTypeId, vp);
            return View(result);
        }
        public IActionResult viewNotes()
        {
            return View();
        }
        //public IActionResult AssignCase()
        //{
        //    AdminDash r = new AdminDash();
        //    r.AdminDashList = new List<AdminDash>();
        //    foreach(var item in result)
        //    {
        //        r.AdminDash.add(new AdminDash
        //        {
        //            Name = item.Name;
        //            RegionId = item.RegionId.ToString();
        //        }
        //    });

        //    return View();
        //}
       
    }
}
