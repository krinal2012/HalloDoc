using Hallodoc.Entity.Models.ViewModel;
using HalloDoc.Entity.Models.ViewModel;
using Microsoft.AspNetCore.Http;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IPatientDash
    {
        public PaginatedViewModel<PatientDashList> PatientList(int id, int page, int pagesize, string sortColumn, string sortOrder);
        public viewProfile viewProfile(int id);
        public void EditProfile(int id, viewProfile vp);
        public void uploadDocument(int id, IFormFile UploadFile);
        public List<viewDocument> viewDocuments(int requestid);
        public viewPatientReq viewMeData(int id);
        public void meRequset(viewPatientReq viewPatientReq);
        public void elseRequset(viewFamilyReq viewFamilyReq);
    }
}
