using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

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
            var data = _IAdminDash.NewRequestData();
            return View(data);
        }
        public IActionResult viewCase(int RequestId)
        {
            var result = _IAdminDash.ViewCaseData(RequestId);
            return View(result);
        }
    }
}
