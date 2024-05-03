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
            TimesheetModel t = new();
            t.TimeSheetData = result;
            t.endDate = endDate;
            t.startDate = startDate;
            return t;
        }
        public bool TimeSheetSave(TimesheetModel model)
        {
            var count = 0;
            Timesheet data = new Timesheet();
            data.StartDate = model.startDate;
            data.EndDate = model.endDate;
            data.IsFinalized = false;
            data.CreatedDate = DateTime.Now;
            _context.Timesheets.Add(data);
            _context.SaveChanges();
            for (var i = model.startDate; i <= model.endDate; i = i.AddDays(1))
            {
                TimesheetDetail detail = new TimesheetDetail();
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
                    var isExist = _context.TimesheetDetails.Any(x => x.Date == detail.Date);
                    if (isExist)
                    {
                        detail.ModifiedDate = DateTime.Now;
                        _context.TimesheetDetails.Update(detail);
                        _context.SaveChanges();
                    }
                    else
                    {
                        detail.CratedDate = DateTime.Now;
                        _context.TimesheetDetails.Add(detail);
                        _context.SaveChanges();
                    }
                }
                count++;
            }
            return true;
        }

    }
}
