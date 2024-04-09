using HalloDoc.Entity.Models.ViewModel;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IScheduling
    {
        public void AddShift(SchedulingData model, List<string?>? chk, string adminId);
        public void ViewShift(int shiftdetailid);
        public void ViewShiftreturn(SchedulingData modal);
        public bool EditShift(SchedulingData modal, string id);
        public bool ViewShiftDelete(SchedulingData modal, string id);
        public List<PhysiciansData> PhysicianOnCall(int? region);
    }
}
