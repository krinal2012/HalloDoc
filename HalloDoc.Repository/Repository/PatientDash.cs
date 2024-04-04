using Hallodoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
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
        public PaginatedViewModel<PatientDashList> PatientList(int id,  int page, int pagesize, string sortColumn, string sortOrder)
        {
            var list = _context.Requests.Include(x => x.RequestWiseFiles).Where(x => x.UserId == id).Select(x => new PatientDashList
            {
                createdDate = x.CreatedDate,
                Status = (status)x.Status,
                RequestId = x.RequestId,
                Fcount = x.RequestWiseFiles.Count()
            }).ToList();
            switch (sortColumn)
            {
                case "Status":
                    list = sortOrder == "false" ? list.OrderByDescending(x => x.Status).ToList() : list.OrderBy(x => x.Status).ToList();
                    break;
                case "CreatedDate":
                    list = sortOrder == "false" ? list.OrderByDescending(x => x.createdDate).ToList() : list.OrderBy(x => x.createdDate).ToList();
                    break;
                default:
                    list = list.OrderByDescending(x => x.createdDate).ToList();
                    break;
            }
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<PatientDashList> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PaginatedViewModel<PatientDashList> viewModel = new PaginatedViewModel<PatientDashList>()
            {
                AdminList = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };
            return viewModel;
        }
        public viewProfile viewProfile(int id)
        {
            User singleUser = _context.Users.FirstOrDefault(u => u.UserId == id);
            viewProfile user = new();
            user.FirstName = singleUser.FirstName;
            user.LastName = singleUser.LastName;
            user.Email = singleUser.Email;
            user.Mobile = singleUser.Mobile;
            user.Street = singleUser.Street;
            user.City = singleUser.City;
            user.State = singleUser.State;
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
            var items = _context.RequestWiseFiles.Include(m => m.Request)
                .Where(x => x.RequestId == requestid).Select(m => new viewDocument
                {
                    uploaddate = m.CreatedDate,   
                    FirstName = m.Request.FirstName,
                    FileName= m.FileName
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
        public viewPatientReq viewMeData(int id)
        {
            var ViewPatientCreateRequest = _context.Users
                              .Where(r => (r.UserId) == id)
                              .Select(r => new viewPatientReq
                              {
                                  FirstName = r.FirstName,
                                  LastName = r.LastName,
                                  Email = r.Email,
                                  Mobile = r.Mobile,
                                  DOB = new DateTime((int)r.IntYear, Convert.ToInt32(r.StrMonth.Trim()), (int)r.IntDate),
                              })
                              .FirstOrDefault();
            return ViewPatientCreateRequest;
        }
        public void meRequset(viewPatientReq viewPatientReq)
        {
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewPatientReq.Email);

            Request.RequestTypeId = 2;
            Request.Status = 1;
            Request.FirstName = viewPatientReq.FirstName;
            Request.LastName = viewPatientReq.LastName;
            Request.UserId = isexist.UserId;
            Request.Email = viewPatientReq.Email;
            Request.PhoneNumber = viewPatientReq.Mobile;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewPatientReq.FirstName;
            Requestclient.LastName = viewPatientReq.LastName;
            Requestclient.Address = viewPatientReq.Street;
            Requestclient.Email = viewPatientReq.Email;
            Requestclient.PhoneNumber = viewPatientReq.Mobile;
            Requestclient.Notes = viewPatientReq.Symptoms;
            Requestclient.IntDate = viewPatientReq.DOB.Day;
            Requestclient.IntYear = viewPatientReq.DOB.Year;
            Requestclient.StrMonth = (viewPatientReq.DOB.Month).ToString();
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewPatientReq.file != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, viewPatientReq.file.FileName);
                //viewPatientReq.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewpatientcreaterequest.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewPatientReq.file.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewPatientReq.file.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
        public void elseRequset(viewFamilyReq viewFamilyReq)
        {
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewFamilyReq.Email);

            Request.RequestTypeId = 3;
            Request.Status = 1;
            Request.UserId = isexist.UserId;
            Request.FirstName = viewFamilyReq.First_name;
            Request.LastName = viewFamilyReq.Last_name;
            Request.Email = viewFamilyReq.Emailid;
            Request.PhoneNumber = viewFamilyReq.Mobileno;
            Request.RelationName = viewFamilyReq.Relation;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewFamilyReq.FirstName;
            Requestclient.LastName = viewFamilyReq.LastName;
            Requestclient.Address = viewFamilyReq.Street;
            Requestclient.Email = viewFamilyReq.Email;
            Requestclient.PhoneNumber = viewFamilyReq.Mobile;
            Requestclient.IntDate = viewFamilyReq.DOB.Day;
            Requestclient.IntYear = viewFamilyReq.DOB.Year;
            Requestclient.StrMonth = (viewFamilyReq.DOB.Month).ToString();
            Requestclient.Notes = viewFamilyReq.Symptoms;
            Requestclient.Address = viewFamilyReq.Street + "," + viewFamilyReq.City + "," + viewFamilyReq.State;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewFamilyReq.file != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, viewFamilyReq.file.FileName);
                //viewPatientReq.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewpatientcreaterequest.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewFamilyReq.file.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewFamilyReq.file.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
    }
}
