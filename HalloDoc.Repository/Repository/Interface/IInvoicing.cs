﻿using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IInvoicing
    {
        public TimesheetModel TimeSheetData(DateTime startDate, DateTime endDate);
        public bool TimeSheetSave(TimesheetModel model);
    }
}
