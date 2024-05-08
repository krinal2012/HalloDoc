using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HalloDoc.Repository.Repository
{
    public class Invoicing : IInvoicing
    {
        private readonly HelloDocContext _context;
        public Invoicing(HelloDocContext context)
        {
            _context = context;
        }
        public TimesheetModel TimeSheetData(DateTime startDate, DateTime endDate)
        {
            var result = _context.TimesheetDetails.Where(r => r.Date >= startDate.Date && r.Date <= endDate.Date).
                ToList();
            var data = _context.TimesheetReciepts.Where(r => r.Date >= startDate.Date && r.Date <= endDate.Date).
                ToList();
            TimesheetModel t = new();
            t.TimeSheetData = result;
            t.TimesheetRecieptData = data;
            t.endDate = endDate;
            t.startDate = startDate;
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
                    return false;
                }
                var timesheetId = timesheet.TimesheetId;
                for (var i = model.startDate; i <= model.endDate; i = i.AddDays(1))
                {
                    var detail = _context.TimesheetDetails.FirstOrDefault(x => x.Date == i && x.TimesheetId == timesheetId);
                    var reciept = _context.TimesheetReciepts.FirstOrDefault(x => x.Date == i && x.TimesheetDetailsId == detail.TimesheetDetailsId);
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
                    if(reciept != null)
                    {
                        reciept.Date = default;
                        if (model.Items[count] != null)
                        {
                            reciept.Date = i;
                            reciept.Item =model.Items[count];
                        }
                        //if (model.Bills[count] != null)
                        //{
                        //    reciept.Date = i;
                        //    reciept.BillName = model.Bills[count];
                        //}
                        if (model.Amount[count] != null)
                        {
                            reciept.Date = i;
                            reciept.Amount = Convert.ToInt32(model.Amount[count]);
                        }
                        if (reciept.Date != default)
                        {
                            reciept.TimesheetDetailsId = detail.TimesheetDetailsId;
                            reciept.ModifiedDate = DateTime.Now;
                            _context.TimesheetReciepts.Update(reciept);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        reciept = new TimesheetReciept();
                        reciept.Date = default;
                        if (model.Items[count] != null)
                        {
                            reciept.Date = i;
                            reciept.Item = model.Items[count];
                        }
                        //if (model.Bills[count] != null)
                        //{
                        //    reciept.Date = i;
                        //    reciept.BillName = model.Bills[count];
                        //}
                        if (model.Amount[count] != null)
                        {
                            reciept.Date = i;
                            reciept.Amount = Convert.ToInt32(model.Amount[count]);
                        }
                        if (reciept.Date != default)
                        {
                            reciept.TimesheetDetailsId = detail.TimesheetDetailsId;
                            reciept.CreatedDate = DateTime.Now;
                            _context.TimesheetReciepts.Add(reciept);
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

    }
}
