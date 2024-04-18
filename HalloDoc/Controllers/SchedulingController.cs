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
using System.Collections;

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
            var PhysiciansByRegion = _IAdminDash.ProviderbyRegion(regionid);

            return Json(PhysiciansByRegion);
        }
     
        //public IActionResult Scheduling()
        //{
        //    ViewBag.Adminname = HttpContext.Session.GetString("Adminname");
        //    ViewBag.RegionCombobox = _adminFunction.RegionComboBox();
        //    ViewBag.PhysiciansByRegion = new SelectList(Enumerable.Empty<SelectListItem>());
        //    SchedulingModel modal = new SchedulingModel();
        //    modal.regions = _context.Regions.ToList();
        //    return View(modal);
        //}

        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _context.PhysicianRegions.Include(u => u.Physician).Where(u => u.RegionId == regionid).Select(u => u.Physician).ToList();

            physician = _context.Physicians.ToList();


            switch (PartialName)
            {

                case "_DayWise":
                    DayWiseScheduling day = new DayWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                        //shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ToList()
                        shiftdetails = _context.ShiftDetailRegions.Include(u => u.ShiftDetail).ThenInclude(u => u.Shift).Where(u => u.RegionId == regionid && u.IsDeleted == new BitArray(new[] { false })).Select(u => u.ShiftDetail).ToList()

                    };
                    if (regionid == 0)
                    {
                        day.shiftdetails = _context.ShiftDetails.Include(u => u.Shift).Where(u => u.IsDeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new WeekWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                        //shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ThenInclude(u => u.Physician).ToList()
                        shiftdetails = _context.ShiftDetailRegions.Include(u => u.ShiftDetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).Where(u => u.RegionId == regionid).Select(u => u.ShiftDetail).ToList()

                    };
                    if (regionid == 0)
                    {
                        week.shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new MonthWiseScheduling
                    {
                        date = currentDate,
                        shiftdetails = _context.ShiftDetailRegions.Include(u => u.ShiftDetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).Where(u => u.RegionId == regionid).Select(u => u.ShiftDetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        month.shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).ToList();
                    }
                    if (Crredntials.Role() == "Physician")
                    {
                        month.shiftdetails = _context.ShiftDetails.Include(u => u.Shift).Where(u => u.IsDeleted == new BitArray(new[] { false }) && u.Shift.PhysicianId == Int32.Parse(Crredntials.UserID())).ToList();
                    }
                    return PartialView("_MonthWise", month);

                default:
                    return PartialView("_DayWise");
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
            SchedulingData modal = new SchedulingData();
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == shiftdetailid);

            if (shiftdetail != null)
            {
                _context.Entry(shiftdetail)
                    .Reference(s => s.Shift)
                    .Query()
                    .Include(s => s.Physician)
                    .Load();
            }
            modal.regionid = (int)shiftdetail.RegionId;
            modal.physicianname = shiftdetail.Shift.Physician.FirstName + " " + shiftdetail.Shift.Physician.LastName;
            modal.modaldate = shiftdetail.ShiftDate.ToString("yyyy-MM-dd");
            modal.starttime = shiftdetail.StartTime;
            modal.endtime = shiftdetail.EndTime;
            modal.shiftdetailid = shiftdetailid;
            return modal;
        }
        public IActionResult ViewShiftreturn(SchedulingData modal)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            if (shiftdetail.Status == 0)
            {
                shiftdetail.Status = 1;
            }
            else
            {
                shiftdetail.Status = 0;
            }
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();

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
        #region RequestedShift
        public async Task<IActionResult> RequestedShift(int? regionId)
        {
            ViewBag.RegionComboBox = _IAdminDash.AssignCase();
            List<SchedulingData> v = await _scheduling.GetAllNotApprovedShift(regionId);

            return View("../Scheduling/ReviewShift", v);
        }
        #endregion

        #region _ApprovedShifts

        public async Task<IActionResult> _ApprovedShifts(string shiftids)
        {
            if (await _scheduling.UpdateStatusShift(shiftids, Crredntials.AspNetUserId()))
            {
                TempData["Status"] = "Approved Shifts Successfully..!";
            }


            return RedirectToAction("RequestedShift");
        }
        #endregion

        #region _DeleteShifts

        public async Task<IActionResult> _DeleteShifts(string shiftids)
        {
            if (await _scheduling.DeleteShift(shiftids, Crredntials.AspNetUserId()))
            {
                TempData["Status"] = "Delete Shifts Successfully..!";
            }

            return RedirectToAction("RequestedShift");
        }
        #endregion
    }
}
