using Hallodoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Repository.Repository.Interface;
using System.Collections;

namespace HalloDoc.Repository.Repository
{
    public class PatientRequest : IPatientRequest
    {
        private readonly HelloDocContext _context;
        public PatientRequest(HelloDocContext context)
        {
            _context = context;
        }
        public  void PatientReq(viewPatientReq viewPatientReq)
        {
            var Aspnetuser = new AspNetUser();
            var User = new User();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewPatientReq.Email);

            if (isexist == null)
            {
                Guid g = Guid.NewGuid();
                Aspnetuser.Id = g.ToString();
                Aspnetuser.UserName = viewPatientReq.FirstName;
                Aspnetuser.PasswordHash = viewPatientReq.Pass;
                Aspnetuser.Email = viewPatientReq.Email;
                Aspnetuser.PhoneNumber = viewPatientReq.Mobile;
                Aspnetuser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(Aspnetuser);
                _context.SaveChanges();
                User.AspNetUserId = Aspnetuser.Id;
                User.FirstName = viewPatientReq.FirstName;
                User.LastName = viewPatientReq.LastName;
                User.Email = viewPatientReq.Email;
                User.Mobile = viewPatientReq.Mobile;
                User.Street = viewPatientReq.Street;
                User.City = viewPatientReq.City;
                User.State = viewPatientReq.State;
                User.ZipCode = viewPatientReq.ZipCode;
                User.StrMonth = viewPatientReq.DOB.Month.ToString();
                User.IntDate = viewPatientReq.DOB.Day;
                User.IntYear = viewPatientReq.DOB.Year;
                User.Status = 1; //for new request
                User.CreatedBy = Aspnetuser.Id;
                User.CreatedDate = DateTime.Now;
                _context.Users.Add(User);
                _context.SaveChanges();
            }
            Request.RequestTypeId = 2;
            Request.Status = 1;
            if (isexist == null)
            {
                Request.UserId = User.UserId;
            }
            else
            {
                Request.UserId = isexist.UserId;
            }
            Request.FirstName = viewPatientReq.FirstName;
            Request.LastName = viewPatientReq.LastName;
            Request.Email = viewPatientReq.Email;
            Request.PhoneNumber = viewPatientReq.Mobile;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            _context.Requests.Add(Request);
             _context.SaveChanges();
            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewPatientReq.FirstName;
            Requestclient.LastName = viewPatientReq.LastName;
            Requestclient.Address = viewPatientReq.Street + "," + viewPatientReq.City + "," + viewPatientReq.State + "," + viewPatientReq.ZipCode;
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
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}
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
        public void FamilyReq(viewFamilyReq viewFamilyReq)
        {
            var Aspnetuser = new AspNetUser();
            var User = new User();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewFamilyReq.Emailid);
            Guid g = Guid.NewGuid();

            //Aspnetuser.Id = g.ToString();
            //Aspnetuser.UserName = viewFamilyReq.FirstName;
            //Aspnetuser.PasswordHash = "krinal";
            //Aspnetuser.Email = viewFamilyReq.Email;
            //Aspnetuser.PhoneNumber = viewFamilyReq.Mobile;
            //Aspnetuser.CreatedDate = DateTime.Now;
            //_context.AspNetUsers.Add(Aspnetuser);
            // _context.SaveChangesAsync();

            //User.AspNetUserId = viewFamilyReq.Id;
            //User.FirstName = viewFamilyReq.FirstName;
            //User.LastName = viewFamilyReq.LastName;
            //User.Email = viewFamilyReq.Email;
            //User.Mobile = viewFamilyReq.Mobile;
            //User.Street = viewFamilyReq.Street;
            //User.City = viewFamilyReq.City;
            //User.State = viewFamilyReq.State;
            //User.ZipCode = viewFamilyReq.ZipCode;
            //User.StrMonth = (viewFamilyReq.DOB.Month).ToString();
            //User.IntDate = viewFamilyReq.DOB.Day;
            //User.IntYear = viewFamilyReq.DOB.Year;
            //User.Status = 1;
            //User.CreatedBy = Aspnetuser.Id;
            //User.CreatedDate = DateTime.Now;
            //_context.Users.Add(User);
            // _context.SaveChangesAsync();

            Request.RequestTypeId = 3;
            Request.Status = 1;
            if (isexist == null)
            {
                Request.UserId = User.UserId;
            }
            else
            {
                Request.UserId = isexist.UserId;
            }
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
            Requestclient.Address = viewFamilyReq.Street + "," + viewFamilyReq.City + "," + viewFamilyReq.State + "," + viewFamilyReq.ZipCode;
            Requestclient.Email = viewFamilyReq.Email;
            Requestclient.PhoneNumber = viewFamilyReq.Mobile;
            Requestclient.IntDate = viewFamilyReq.DOB.Day;
            Requestclient.IntYear = viewFamilyReq.DOB.Year;
            Requestclient.StrMonth = (viewFamilyReq.DOB.Month).ToString();
            Requestclient.Notes = viewFamilyReq.Symptoms;           
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
        public void BusinessReq(viewBusinessReq viewBusinessReq)
        {
            var Aspnetuser = new AspNetUser();
            var User = new User();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var Business = new Business();
            var Requestbusiness = new RequestBusiness();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewBusinessReq.Emailid);
            //Guid g = Guid.NewGuid();
            //Aspnetuser.Id = g.ToString();
            //Aspnetuser.UserName = viewBusinessReq.FirstName;
            //Aspnetuser.PasswordHash = viewBusinessReq.FirstName;
            //Aspnetuser.Email = viewBusinessReq.Email;
            //Aspnetuser.PhoneNumber = viewBusinessReq.Mobile;
            //Aspnetuser.CreatedDate = DateTime.Now;
            //_context.AspNetUsers.Add(Aspnetuser);
            // _context.SaveChangesAsync();

            //User.AspNetUserId = viewBusinessReq.Id;
            //User.FirstName = viewBusinessReq.FirstName;
            //User.LastName = viewBusinessReq.LastName;
            //User.Email = viewBusinessReq.Email;
            //User.Mobile = viewBusinessReq.Mobile;
            //User.Street = viewBusinessReq.Street;
            //User.City = viewBusinessReq.City;
            //User.State = viewBusinessReq.State;
            //User.ZipCode = viewBusinessReq.ZipCode;
            //User.StrMonth = (viewBusinessReq.DOB.Month).ToString();
            //User.IntDate = viewBusinessReq.DOB.Day;
            //User.IntYear = viewBusinessReq.DOB.Year;
            //User.Status = 1;
            //User.CreatedBy = Aspnetuser.Id;
            //User.CreatedDate = DateTime.Now;
            //_context.Users.Add(User);
            // _context.SaveChangesAsync();

            Request.RequestTypeId = 1;
            Request.Status = 1;
            if (isexist == null)
            {
                Request.UserId = User.UserId;
            }
            else
            {
                Request.UserId = isexist.UserId;
            }
            Request.FirstName = viewBusinessReq.First_name;
            Request.LastName = viewBusinessReq.Last_name;
            Request.Email = viewBusinessReq.Emailid;
            Request.PhoneNumber = viewBusinessReq.Mobileno;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewBusinessReq.FirstName;
            Requestclient.LastName = viewBusinessReq.LastName;
            Requestclient.Address = viewBusinessReq.Street + "," + viewBusinessReq.City + "," + viewBusinessReq.State + "," + viewBusinessReq.ZipCode;
            Requestclient.Email = viewBusinessReq.Email;
            Requestclient.PhoneNumber = viewBusinessReq.Mobile;
            Requestclient.IntDate = viewBusinessReq.DOB.Day;
            Requestclient.IntYear = viewBusinessReq.DOB.Year;
            Requestclient.StrMonth = (viewBusinessReq.DOB.Month).ToString();
            Requestclient.Notes = viewBusinessReq.Symptoms;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            Business.Name = viewBusinessReq.First_name + " " + viewBusinessReq.Last_name;
            Business.Address1 = viewBusinessReq.Businessname;
            Business.CreatedDate = DateTime.Now;
            Business.CreatedBy = isexist.AspNetUserId;
            Business.PhoneNumber = viewBusinessReq.Mobileno;
            _context.Businesses.Add(Business);
            _context.SaveChanges();

            Requestbusiness.RequestId = Request.RequestId;
            Requestbusiness.BusinessId = Business.BusinessId;
            _context.RequestBusinesses.Add(Requestbusiness);
            _context.SaveChanges();

        }
        public void ConciergeReq(viewConciergeReq viewConciergeReq)
        {
            var Aspnetuser = new AspNetUser();
            var User = new User();
            var Request = new Request();
            var Requestclient = new RequestClient();
            var Concierge = new Concierge();
            var Requestconcierge = new RequestConcierge();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewConciergeReq.Emailid);
            Guid g = Guid.NewGuid();

            //Aspnetuser.Id = g.ToString();
            //Aspnetuser.UserName = viewConciergeReq.FirstName;
            //Aspnetuser.PasswordHash = viewConciergeReq.FirstName;
            //Aspnetuser.Email = viewConciergeReq.Email;
            //Aspnetuser.PhoneNumber = viewConciergeReq.Mobile;
            //Aspnetuser.CreatedDate = DateTime.Now;
            //_context.AspNetUsers.Add(Aspnetuser);
            // _context.SaveChangesAsync();
            ////for user table
            //User.AspNetUserId = viewConciergeReq.Id;
            //User.FirstName = viewConciergeReq.FirstName;
            //User.LastName = viewConciergeReq.LastName;
            //User.Email = viewConciergeReq.Email;
            //User.Mobile = viewConciergeReq.Mobile;
            //User.Street = viewConciergeReq.Street;
            //User.City = viewConciergeReq.City;
            //User.State = viewConciergeReq.State;
            //User.ZipCode = viewConciergeReq.ZipCode;
            //User.StrMonth = (viewConciergeReq.DOB.Month).ToString();
            //User.IntDate = viewConciergeReq.DOB.Day;
            //User.IntYear = viewConciergeReq.DOB.Year;
            //User.Status = 1;
            //User.CreatedBy = Aspnetuser.Id;
            //User.CreatedDate = DateTime.Now;
            //_context.Users.Add(User);
            // _context.SaveChangesAsync();

            Request.RequestTypeId = 4;
            Request.Status = 1;
            if (isexist == null)
            {
                Request.UserId = User.UserId;
            }
            else
            {
                Request.UserId = isexist.UserId;
            }
            Request.FirstName = viewConciergeReq.First_name;
            Request.LastName = viewConciergeReq.Last_name;
            Request.Email = viewConciergeReq.Emailid;
            Request.PhoneNumber = viewConciergeReq.Mobileno;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewConciergeReq.FirstName;
            Requestclient.LastName = viewConciergeReq.LastName;
            Requestclient.Address = viewConciergeReq.Street + "," + viewConciergeReq.City + "," + viewConciergeReq.State + "," + viewConciergeReq.ZipCode;
            Requestclient.Email = viewConciergeReq.Email;
            Requestclient.PhoneNumber = viewConciergeReq.Mobile;
            Requestclient.IntDate = viewConciergeReq.DOB.Day;
            Requestclient.IntYear = viewConciergeReq.DOB.Year;
            Requestclient.StrMonth = (viewConciergeReq.DOB.Month).ToString();
            Requestclient.Notes = viewConciergeReq.Symptoms;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            Concierge.ConciergeName = viewConciergeReq.First_name + "" + viewConciergeReq.Last_name;
            Concierge.Street = viewConciergeReq.Street;
            Concierge.City = viewConciergeReq.City;
            Concierge.State = viewConciergeReq.State;
            Concierge.ZipCode = viewConciergeReq.ZipCode;
            Concierge.CreatedDate = DateTime.Now;
            _context.Concierges.Add(Concierge);
            _context.SaveChanges();

            Requestconcierge.RequestId = Request.RequestId;
            Requestconcierge.ConciergeId = Concierge.ConciergeId;
            _context.RequestConcierges.Add(Requestconcierge);
            _context.SaveChanges();

        }
    }
}

    
