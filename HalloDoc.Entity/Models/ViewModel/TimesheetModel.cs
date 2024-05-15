
using HalloDoc.Entity.DataModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class TimesheetModel
    {
        public string DateRange { get; set; }
        public bool isFinalized { get; set; } = false;
        public int TimesheetId { get; set; }
        public int PhysicianId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public IFormFile Bill { get; set; }
        public List<TimesheetDetail> TimeSheetData { get; set; }
        public List<TimesheetReciept> TimesheetRecieptData { get; set; }
        public PhysicianPayrate PhysicianPayrateData { get; set; }
        public List<string> OnCallHours { get; set; }
        public List<string> TotalHours { get; set; }
        public List<bool> IsWeekend { get; set; }
        public List<string> NoofHousecall { get; set; }
        public List<string> NoofPhoneConsult { get; set; }
      

    }
}
