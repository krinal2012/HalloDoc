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

namespace HalloDoc.Repository.Repository
{
    public class Invoicing: IInvoicing
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

    }
}
