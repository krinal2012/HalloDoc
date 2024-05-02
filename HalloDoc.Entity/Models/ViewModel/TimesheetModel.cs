
using HalloDoc.Entity.DataModels;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class TimesheetModel
    {
        public string DateRange { get; set; }
        public bool isFinalize { get; set; } = false;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<TimesheetDetail> TimeSheetData { get; set; }
        public int? OnCallHours { get; set; }

        public int? TotalHours { get; set; }

        public bool? IsWeekend { get; set; }

        public int? NoofHousecall { get; set; }

        public int? NoofPhoneConsult { get; set; }

    }
}
