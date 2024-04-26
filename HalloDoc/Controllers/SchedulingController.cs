﻿using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.DataContext;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HalloDoc.Repository.Repository.Interface;
using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Entity.Models.ViewModel;

namespace HalloDoc.Controllers
{
    public class SchedulingController : Controller
    {
        private readonly IAdminDash _IAdminDash;
        private readonly HelloDocContext _context;
        private readonly INotyfService _notyf;
        private readonly IScheduling _scheduling;
        public SchedulingController(IAdminDash IAdminDash, HelloDocContext context, INotyfService notyf, IScheduling scheduling)
        {
            _IAdminDash = IAdminDash;
            _context = context;
            _notyf = notyf;
            _scheduling = scheduling;
        }
        public async Task<IActionResult> Index(int? region)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.PhysiciansByRegion = new SelectList(Enumerable.Empty<SelectListItem>());
            SchedulingData modal = new SchedulingData();
            return View("../Scheduling/Index", modal);

        }
        public IActionResult GetPhysicianByRegion(int regionid)
        {
            var ProviderbyRegion = _IAdminDash.ProviderbyRegion(regionid);
            return Json(ProviderbyRegion);
        }
        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _context.PhysicianRegions.Include(u => u.Physician).Where(u => u.RegionId == regionid).Select(u => u.Physician).ToList();
            physician = _context.Physicians.ToList();
            switch (PartialName)
            {
                case "_DayWise":
                    return PartialView("_DayWise", _scheduling.Daywise(regionid, currentDate));

                case "_WeekWise":
                    return PartialView("_WeekWise", _scheduling.Weekwise(regionid, currentDate));

                case "_MonthWise":
                    
                    if (Crredntials.Role() == "Physician")
                    {
                        return PartialView("_MonthWise", _scheduling.Monthwise(regionid, currentDate,Int32.Parse(Crredntials.UserID())));
                    }
                    else
                    {
                        return PartialView("_MonthWise", _scheduling.Monthwise(regionid, currentDate, Int32.Parse(Crredntials.UserID())));
                    }
                       
                default:
                    return PartialView("_MonthWise", _scheduling.Monthwise(regionid, currentDate, Int32.Parse(Crredntials.UserID())));
            }
        }
        public IActionResult AddShift(SchedulingData model)
        {
            string adminId = Crredntials.AspNetUserId();
            var chk = Request.Form["repeatdays"].ToList();
            if (model.physicianid == 0)
            {
                model.physicianid = Int32.Parse(Crredntials.UserID());
            }
            _scheduling.AddShift(model, chk, adminId);
            return RedirectToAction("Index");
        }
        public SchedulingData viewshift(int shiftdetailid)
        {
            return _scheduling.ViewShift(shiftdetailid);
        }
        public IActionResult ViewShiftreturn(SchedulingData modal)
        {
            if (modal.shiftdate.Date < DateTime.Today.Date)
            {
                _notyf.Warning("Cannot edit old shifts");
                return RedirectToAction("Index");
            }
            _scheduling.ViewShiftreturn(modal);
            return RedirectToAction("Index");

        }
        public void ViewShiftSave(SchedulingData modal)
        {
            _scheduling.EditShift(modal, Crredntials.AspNetUserId());
        }
        public IActionResult ViewShiftDelete(SchedulingData modal)
        {
            _scheduling.ViewShiftDelete(modal, Crredntials.AspNetUserId());
            return RedirectToAction("Index");
        }
        public IActionResult MDSOnCall(int? regionId)
        {
            ViewBag.AssignCase =  _IAdminDash.AssignCase();
            List<PhysiciansData> v =  _scheduling.PhysicianOnCall(regionId);
            if (regionId != null)
            {
                return Json(v);
            }
            return View("../Scheduling/MDSOnCall", v);
        }
        public async Task<IActionResult> RequestedShift(int? regionId)
        {
            ViewBag.RegionComboBox = _IAdminDash.AssignCase();
            List<SchedulingData> v = await _scheduling.GetAllNotApprovedShift(regionId);
            return View("../Scheduling/ReviewShift", v);
        }
        public async Task<IActionResult> _ApprovedShifts(string shiftids)
        {
            if (await _scheduling.UpdateStatusShift(shiftids, Crredntials.AspNetUserId()))
            {
                TempData["Status"] = "Approved Shifts Successfully..!";
            }
            return RedirectToAction("RequestedShift");
        }
        public async Task<IActionResult> _DeleteShifts(string shiftids)
        {
            if (await _scheduling.DeleteShift(shiftids, Crredntials.AspNetUserId()))
            {
                TempData["Status"] = "Delete Shifts Successfully..!";
            }
            return RedirectToAction("RequestedShift");
        }
      
    }
}
