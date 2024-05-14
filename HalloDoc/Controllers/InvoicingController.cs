using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Models;
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
            ViewBag.Physician = _Invoicing.GetAllPhysician();
            return View();
        }
        public IActionResult TimeSheetData(string startDate, string endDate,int PhysicianId)
        {
            if (PhysicianId == 0)
            {
                PhysicianId = Convert.ToInt32(Crredntials.UserID());
            }
            var provider = CultureInfo.InvariantCulture;
            DateTime sd = DateTime.ParseExact(startDate, "dd/MM/yyyy", provider);
            DateTime ed = DateTime.ParseExact(endDate, "dd/MM/yyyy", provider);
            var res = _Invoicing.TimeSheetData(sd, ed,PhysicianId);
            return PartialView("_Timesheet", res);
        }

        public IActionResult FinalizeTime(string startDate, string endDate, int PhysicianId)
        {
            var provider = CultureInfo.InvariantCulture;
            DateTime sd = DateTime.ParseExact(startDate, "dd/MM/yyyy", provider);
            DateTime ed = DateTime.ParseExact(endDate, "dd/MM/yyyy", provider);
            var res = _Invoicing.TimeSheetData(sd, ed,PhysicianId);
            return View(res);
        }

        [HttpPost]
        public IActionResult TimeSheetSave(TimesheetModel sendInfo)
        {
            var res = _Invoicing.TimeSheetSave(sendInfo);
            return RedirectToAction("FinalizeTime", new { sendInfo.startDate, sendInfo.endDate });
        }
       
        [HttpPost]
        public IActionResult RecieptSave(TimesheetModel formData)
        {
            var res = _Invoicing.TimeSheetRecieptSave(formData);
            return RedirectToAction("FinalizeTime", new { formData.startDate, formData.endDate });
        }
        public IActionResult FinalizeTimesheet(int timesheetId)
        {
            bool res = _Invoicing.FinalizeTimesheet(timesheetId);
            return RedirectToAction("FinalizeTime");

        }
        public IActionResult ApproveTimesheet(TimesheetModel formData)
        {
            bool res = _Invoicing.ApproveTimesheet(formData);
            return RedirectToAction("FinalizeTime");

        }

    }
}
