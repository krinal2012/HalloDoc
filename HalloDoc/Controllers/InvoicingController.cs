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
        private readonly HelloDocContext _context;
        private readonly INotyfService _notyf;

        public InvoicingController(IInvoicing Invoicing, HelloDocContext context, INotyfService notyf)
        {
            _Invoicing = Invoicing;
            _context = context;
            _notyf = notyf;
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

        public IActionResult FinalizeTime(int TimesheetId, int PhysicianId)
        {
            if (PhysicianId == 0)
            {
                PhysicianId = Convert.ToInt32(Crredntials.UserID());
            }
            var sd = _context.Timesheets.Where(r => r.TimesheetId == TimesheetId).Select(r => r.StartDate).FirstOrDefault();
            var ed = _context.Timesheets.Where(r => r.TimesheetId == TimesheetId).Select(r => r.EndDate).FirstOrDefault();
            var provider = CultureInfo.InvariantCulture;
            //DateTime sd = DateTime.ParseExact(startDate, "dd/MM/yyyy", provider);
            //DateTime ed = DateTime.ParseExact(endDate, "dd/MM/yyyy", provider);
            var res = _Invoicing.TimeSheetData(sd, ed,PhysicianId);
            return View(res);
        }

        [HttpPost]
        public IActionResult TimeSheetSave(TimesheetModel sendInfo)
        {
            var res = _Invoicing.TimeSheetSave(sendInfo);
            if (res)
            {
                _notyf.Success("Saved Successfully");
            }
            else
            {
                _notyf.Error("Data not Saved");
            }
            return RedirectToAction("FinalizeTime", new { sendInfo.TimesheetId , sendInfo.PhysicianId});
        }
       
        [HttpPost]
        public IActionResult RecieptSave(TimesheetModel formData)
        {
            var res = _Invoicing.TimeSheetRecieptSave(formData);
            return RedirectToAction("FinalizeTime", new { formData.TimesheetId, formData.PhysicianId });
        }
        public IActionResult FinalizeTimesheet(int timesheetId, int physicianId)
        {
            bool res = _Invoicing.FinalizeTimesheet(timesheetId);
            return RedirectToAction("FinalizeTime", new { timesheetId, physicianId });

        }
        public IActionResult ApproveTimesheet(TimesheetModel formData)
        {
            bool res = _Invoicing.ApproveTimesheet(formData);
            return RedirectToAction("FinalizeTime", new { formData.TimesheetId, formData.PhysicianId });

        }

    }
}
