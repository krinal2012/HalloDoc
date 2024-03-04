
using System.Web.Mvc;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminDash
    {
        public CountStatusWiseRequestModel CountRequestData();
        public List<AdminList> NewRequestData(int statusid, string? searchValue);
        public ViewCaseModel ViewCaseData(int RequestID, int RequestTypeId);
        public ViewCaseModel EditViewCaseData(int RequestID, int RequestTypeId, ViewCaseModel vp);
        public List<Physician> ProviderbyRegion(int Regionid);
        public List<Region> AssignCase();
        public void AssignCaseInfo(int RequestId, int PhysicianId, string Notes);
        public List<CaseTag> CaseReason();
        public bool CancleCaseInfo(int? RequestId, string caseTag, string Notes);
        public bool BlockCaseInfo(int RequestId, string Notes);
        
    }
}
