
using HalloDoc.Entity.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class TimesheetModel
    {
        public string DateRange { get; set; }
        public bool isFinalize { get; set; } = false;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<TimesheetDetail> TimeSheetData { get; set; }
        public List<TimesheetReciept> TimesheetRecieptData { get; set; }
        public List<string> OnCallHours { get; set; }
        public List<string> TotalHours { get; set; }
        public List<bool> IsWeekend { get; set; }
        public List<string> NoofHousecall { get; set; }
        public List<string> NoofPhoneConsult { get; set; }
        public List<string> Items { get; set; }
        public List<string> Bills { get; set; }
        public List<int> Amount { get; set; }

    }
}
