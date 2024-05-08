using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Web.WebPages;

namespace HalloDoc.Controllers
{
    public class InvoicingController : Controller
    {
        private readonly IInvoicing _Invoicing;
        public InvoicingController(IInvoicing Invoicing)
        {
            _Invoicing = Invoicing;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult FinalizeTime(string startDate, string endDate)
        {
            var provider = CultureInfo.InvariantCulture;
            DateTime sd = DateTime.ParseExact(startDate, "dd/MM/yyyy", provider);
            DateTime ed = DateTime.ParseExact(endDate, "dd/MM/yyyy", provider);
            var res = _Invoicing.TimeSheetData(sd, ed);
            return View(res);
        }

        [HttpPost]
        public IActionResult TimeSheetSave(TimesheetModel sendInfo)
        {
            var res = _Invoicing.TimeSheetSave(sendInfo);
            return RedirectToAction("FinalizeTime", new { sendInfo.startDate, sendInfo.endDate });
        }
    }
}
