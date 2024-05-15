using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IInvoicing
    {
        public TimesheetModel TimeSheetData(DateTime? startDate, DateTime? endDate, int PhysicianId);
        public bool TimeSheetSave(TimesheetModel model);
        public bool TimeSheetRecieptSave(TimesheetModel model);
        public bool FinalizeTimesheet(int timesheetId);
        public List<Physician> GetAllPhysician();
        public bool ApproveTimesheet(TimesheetModel formData);
    }
}
