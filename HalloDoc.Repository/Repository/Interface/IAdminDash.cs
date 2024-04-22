using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataModels;
using Microsoft.AspNetCore.Http;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminDash
    {
        public CountStatusWiseRequestModel CountRequestData(int phyid);
        public PaginatedViewModel<AdminList> NewRequestData(int userid,int statusid, string? searchValue, int page, int pagesize, int? Region, string sortColumn, string sortOrder, int? requesttype);
        public ViewCaseModel ViewCaseData(int RequestID, int RequestTypeId, int status);
        public ViewCaseModel EditViewCaseData(int RequestID, int RequestTypeId, ViewCaseModel vp);
        public List<Physician> ProviderbyRegion(int Regionid);
        public List<Region> AssignCase();
        public List<AspNetRole> AspNetRole();
        public bool AcceptCase(int RequestId, string Notes, int PhysicianId);
        public bool TransferCase(int RequestId, string Notes, int PhysicianId);
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
        public Task<bool> SendFileEmail(string ids, int Requestid, string email);
        public bool Delete(int id, int[] requestfileid);
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
        public bool EditEncounterinfo(ViewEncounterForm ve);
        public bool Finalizeform(ViewEncounterForm ve);
        public bool SendLink(sendAgreement sendAgreement);
        public bool CreateReq(viewPatientReq viewPatientReq, string UserId);
        public List<AdminList> Export(string status);
        public bool SendMessage(string? Message);
        public bool Housecall(int RequestId);
        public bool Consult(int RequestId);
        public bool ConcludeCare(int RequestId, string Notes);
        public bool IsEncounterFinalized(int requestId);


    }
}
