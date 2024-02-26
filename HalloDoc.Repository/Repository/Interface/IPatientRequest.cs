using Hallodoc.Entity.Models.ViewModel;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IPatientRequest
    {
        public void PatientReq(viewPatientReq viewPatientReq);
        public void FamilyReq(viewFamilyReq viewFamilyReq);
        public void BusinessReq(viewBusinessReq viewBusinessReq);
        public void ConciergeReq(viewConciergeReq viewConciergeReq);

    }
}
