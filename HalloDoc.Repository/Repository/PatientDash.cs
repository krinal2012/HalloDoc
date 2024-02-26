using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Repository.Repository
{
    public class PatientDash : IPatientDash
    {
        private readonly HelloDocContext _context;
        public PatientDash(HelloDocContext context)
        {
            _context = context;
        }

        public List<PatientDashList> PatientList(int id)
        {
            var items = _context.Requests.Include(x => x.RequestWiseFiles).Where(x => x.UserId == id).Select(x => new PatientDashList
            {
                 createdDate = x.CreatedDate,
                 Status = (status)x.Status,
                 RequestId = x.RequestId,
                 Fcount = x.RequestWiseFiles.Count()

             }).ToList();
        return items;
        }

        public viewProfile viewProfile(int id)
        {
            User singleUser = _context.Users.FirstOrDefault(u => u.UserId == id);
            viewProfile user = new viewProfile();

            user.FirstName = singleUser.FirstName;
            user.LastName = singleUser.LastName;
            user.Email = singleUser.Email;
            user.Mobile = singleUser.Mobile;
            user.Street = singleUser.Street;
            user.City = singleUser.City;
            user.State= singleUser.State;

            return user;

        }
        public void EditProfile(int id, viewProfile vp)
        {
            var userToUpdate = _context.Users.FirstOrDefault(x => x.UserId == id);

            if (userToUpdate != null)
            {

                userToUpdate.FirstName = vp.FirstName;
                userToUpdate.LastName = vp.LastName;
                userToUpdate.Mobile = vp.Mobile;
                userToUpdate.Email = vp.Email;
                userToUpdate.State = vp.State;
                userToUpdate.Street = vp.Street;
                userToUpdate.City = vp.City;
                userToUpdate.ZipCode = vp.Zipcode;
                userToUpdate.IntDate = vp.DOB.Day;
                userToUpdate.IntYear = vp.DOB.Year;
                userToUpdate.StrMonth = vp.DOB.Month.ToString();
                userToUpdate.ModifiedBy = vp.Createdby;
                userToUpdate.ModifiedDate = DateTime.Now;
                _context.Update(userToUpdate);
                 _context.SaveChanges();
            }
           
        }
        public List<viewDocument> viewDocuments(int requestid)
        {
            var items = _context.RequestWiseFiles.Where(x => x.RequestId == requestid).Select(m => new viewDocument
            {
                uploaddate = m.CreatedDate,
                uploader = m.FileName
            }).ToList();
            return items;
        }
        public void uploadDocument(int RequestId, IFormFile UploadFile)
        {
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = RequestId,
                    FileName = UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }

        }
    }
}
