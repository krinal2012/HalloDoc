using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDoc.Repository.Repository
{
    public class Invoicing : IInvoicing
    {
        private readonly HelloDocContext _context;
        public Invoicing(HelloDocContext context)
        {
            _context = context;
        }
        public TimesheetModel TimeSheetData(DateTime startDate, DateTime endDate,int PhysicianId)
        {
            var timesheetId = _context.Timesheets.Where(r => r.StartDate == startDate && r.EndDate == endDate && r.PhysicianId==PhysicianId).Select(r=> r.TimesheetId).FirstOrDefault();
            var result = _context.TimesheetDetails.Where(r => r.TimesheetId==timesheetId).ToList();
            var data = _context.TimesheetReciepts.Where(r => r.TimesheetId == timesheetId).ToList();
            var payratedata = _context.PhysicianPayrates.Where(r => r.PhysicianId == PhysicianId).FirstOrDefault();

            TimesheetModel t = new()
            {
                TimeSheetData = result,
                TimesheetRecieptData = data,
                endDate = endDate,
                startDate = startDate,
                TimesheetId = timesheetId,
                PhysicianPayrateData=payratedata
            };
            return t;
        }
        public bool TimeSheetSave(TimesheetModel model)
        {
            var count = 0;
            try
            {
                var timesheet = _context.Timesheets
                .FirstOrDefault(r => r.StartDate == model.startDate && r.EndDate == model.endDate);
                if (timesheet == null)
                {
                    timesheet = new Timesheet
                    {
                        StartDate = model.startDate,
                        EndDate = model.endDate,
                        IsFinalized = false,
                        CreatedDate = DateTime.Now
                    };
                    _context.Timesheets.Add(timesheet);
                    _context.SaveChanges();
                }
                var timesheetId = timesheet.TimesheetId;
                for (var i = model.startDate; i <= model.endDate; i = i.AddDays(1))
                {
                    var detail = _context.TimesheetDetails.FirstOrDefault(x => x.Date == i && x.TimesheetId == timesheetId);
                    if (detail != null)
                    {
                        detail.Date = default;
                        if (model.TotalHours[count] != null)
                        {
                            detail.Date = i;
                            detail.TotalHours = Convert.ToInt32(model.TotalHours[count]);
                        }
                        if (model.IsWeekend[count] != false)
                        {
                            detail.Date = i;
                            detail.IsWeekend = model.IsWeekend[count];
                        }
                        if (model.NoofPhoneConsult[count] != null)
                        {
                            detail.Date = i;
                            detail.NoofPhoneConsult = Convert.ToInt32(model.NoofPhoneConsult[count]);
                        }
                        if (model.NoofHousecall[count] != null)
                        {
                            detail.Date = i;
                            detail.NoofHousecall = Convert.ToInt32(model.NoofHousecall[count]);
                        }
                        if (detail.Date != default)
                        {
                            detail.TimesheetId = timesheetId;
                            detail.ModifiedDate = DateTime.Now;
                            _context.TimesheetDetails.Update(detail);
                            _context.SaveChanges();
                        }

                    }
                    else
                    {
                        detail = new TimesheetDetail();
                        detail.Date = default;
                        if (model.TotalHours[count] != null)
                        {
                            detail.Date = i;
                            detail.TotalHours = Convert.ToInt32(model.TotalHours[count]);
                        }
                        if (model.IsWeekend[count] != false)
                        {
                            detail.Date = i;
                            detail.IsWeekend = model.IsWeekend[count];
                        }
                        if (model.NoofPhoneConsult[count] != null)
                        {
                            detail.Date = i;
                            detail.NoofPhoneConsult = Convert.ToInt32(model.NoofPhoneConsult[count]);
                        }
                        if (model.NoofHousecall[count] != null)
                        {
                            detail.Date = i;
                            detail.NoofHousecall = Convert.ToInt32(model.NoofHousecall[count]);
                        }
                        if (detail.Date != default)
                        {
                            detail.TimesheetId = timesheetId;
                            detail.CratedDate = DateTime.Now;
                            _context.TimesheetDetails.Add(detail);
                            _context.SaveChanges();
                        }

                    }

                    count++;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
        public bool TimeSheetRecieptSave(TimesheetModel model)
        {
            try
            {
                var data = model.TimesheetRecieptData.FirstOrDefault();
                var reciept = _context.TimesheetReciepts.FirstOrDefault(x => x.Date == data.Date); ;

                if (reciept != null)
                {
                    reciept.Date = default;
                    if (data.Item != null)
                    {
                        reciept.Date = data.Date;
                        reciept.Item = data.Item;
                    }
                    if (data.BillName != null)
                    {
                        reciept.Date = data.Date;
                        string FilePath = "wwwroot\\Upload\\Reciept";
                        string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                        string fileNameWithPath = Path.Combine(path, model.Bill.FileName);

                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            model.Bill.CopyTo(stream);
                        }

                        reciept.BillName = model.Bill.FileName;
                    }
                    if (data.Amount != null)
                    {
                        reciept.Date = data.Date;
                        reciept.Amount = Convert.ToInt32(data.Amount);
                    }
                    if (reciept.Date != default)
                    {
                        reciept.ModifiedDate = DateTime.Now;
                        _context.TimesheetReciepts.Update(reciept);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    reciept = new TimesheetReciept();
                    reciept.Date = default;
                    if (data.Item != null)
                    {
                        reciept.Date = data.Date;
                        reciept.Item = data.Item;
                    }
                    if (data.BillName != null)
                    {
                        reciept.Date = data.Date;
                        string FilePath = "wwwroot\\Upload\\Reciept";
                        string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                        string fileNameWithPath = Path.Combine(path, model.Bill.FileName);

                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            model.Bill.CopyTo(stream);
                        }

                        reciept.BillName = model.Bill.FileName;
                    }
                    if (data.Amount != null)
                    {
                        reciept.Date = data.Date;
                        reciept.Amount = Convert.ToInt32(data.Amount);
                    }
                    if (reciept.Date != default)
                    {
                        reciept.CreatedDate = DateTime.Now;
                        _context.TimesheetReciepts.Add(reciept);
                        _context.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
        public bool FinalizeTimesheet(int timesheetId)
        {
            try
            {
                var data = _context.Timesheets.Where(e => e.TimesheetId == timesheetId).FirstOrDefault();
                if (data != null)
                {
                    data.IsFinalized = true;
                    data.ModifiedDate = DateTime.Now;
                    _context.Timesheets.Update(data);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ApproveTimesheet(TimesheetModel formData)
        {
            try
            {
                var data = _context.Timesheets.Where(e => e.TimesheetId == formData.TimesheetId).FirstOrDefault();
                if (data != null)
                {
                    data.IsApproved = true;
                    data.ModifiedDate = DateTime.Now;
                    _context.Timesheets.Update(data);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<Physician> GetAllPhysician()
        {
            var res = _context.Physicians.Select(x => new Physician
            {
               PhysicianId= x.PhysicianId,
               FirstName=x.FirstName,
               LastName=x.LastName,
            }).ToList();
            return res;
        }

    }
}
