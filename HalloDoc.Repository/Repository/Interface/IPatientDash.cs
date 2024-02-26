using HalloDoc.Entity.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace HalloDoc.Repository.Repository.Interface
{
    public interface IPatientDash
    {
        public List<PatientDashList> PatientList(int id);
        public viewProfile viewProfile(int id);
        public void EditProfile(int id, viewProfile vp);
        public void uploadDocument(int id, IFormFile UploadFile);
        public List<viewDocument> viewDocuments(int requestid);
    }
}
