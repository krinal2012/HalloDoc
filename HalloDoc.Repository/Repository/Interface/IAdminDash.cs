using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using Microsoft.AspNetCore.Http;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminDash
    {
        public CountStatusWiseRequestModel CountRequestData();
        public PaginatedViewModel<AdminList> NewRequestData(int statusid, string? searchValue, int page, int pagesize, int? Region, string sortColumn, string sortOrder, int? requesttype);
        public ViewCaseModel ViewCaseData(int RequestID, int RequestTypeId);
        public ViewCaseModel EditViewCaseData(int RequestID, int RequestTypeId, ViewCaseModel vp);
        public List<Physician> ProviderbyRegion(int Regionid);
        public List<Region> AssignCase();
        public void AssignCaseInfo(int RequestId, int PhysicianId, string Notes);
        public void TransferCaseInfo(int RequestId, int PhysicianId, string Notes);
        public viewNotesData viewNotesData(int RequestId);
        public bool ViewNotes(string? adminnotes, string? physiciannotes, int RequestID);
        public List<CaseTag> CaseReason();
        public bool CancleCaseInfo(int? RequestId, string caseTag, string Notes);
        public bool BlockCaseInfo(int RequestId, string Notes);
        public viewDocument ViewUploadsInfo(int requestid);
        public bool ViewUploadPost(viewDocument v, int userid, IFormFile UploadFile);
        public void DeleteFile(int id);
        public List<HealthProfessionalType> Professions(int RequestId );
        public List<HealthProfessional> VendorByProfession(int Professionid);
        public HealthProfessional SendOrdersInfo(int selectedValue);
        public bool SendOrders(int Requestid, OrderDetail data, string Notes);
        public bool ClearCaseInfo(int? RequestId);
        public bool SendAgreement(sendAgreement sendAgreement);
        public Boolean SendAgreement_accept(int RequestID);
        public Boolean SendAgreement_Reject(int RequestID, string Notes);
        public ViewCaseModel CloseCaseData(int RequestID);
        public bool EditCloseCase(ViewCaseModel vp, int RequestID);
        public bool CloseCase(int RequestID);
        public ViewEncounterForm EncounterInfo(int RequestId);
        public void EditEncounterinfo(ViewEncounterForm ve);
        public bool Finalizeform(ViewEncounterForm ve);
    }
}
