using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Repository.Repository.Interface;
using System.Collections;
using HalloDoc.Entity.Models;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Repository.Repository
{
    public class PatientRequest : IPatientRequest
    {
        private readonly HelloDocContext _context;
        private readonly EmailConfiguration _emailConfig;
        public PatientRequest(HelloDocContext context,EmailConfiguration emailConfiguration)
        {
            _context = context;
            _emailConfig = emailConfiguration;
        }
        public  void PatientReq(viewPatientReq viewPatientReq)
        {
            var Aspnetuser = new AspNetUser();
            var role = new AspNetUserRole();
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
                role.UserId = Aspnetuser.Id;
                role.RoleId = "1";
                _context.AspNetUserRoles.Add(role);
                _context.SaveChanges();
                User.AspNetUserId = Aspnetuser.Id;
                User.FirstName = viewPatientReq.FirstName;
                User.LastName = viewPatientReq.LastName;
                User.Email = viewPatientReq.Email;
                User.Mobile = viewPatientReq.Mobile;
                User.Street = viewPatientReq.Street;
                User.City = viewPatientReq.City;
                User.State = Enum.GetName(typeof(RegionList), viewPatientReq.State);
                User.RegionId = viewPatientReq.State;
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
            if (isexist != null)
            {
                Request.UserId = isexist.UserId;
            }
           
            Request.FirstName = viewPatientReq.FirstName;
            Request.LastName = viewPatientReq.LastName;
            Request.Email = viewPatientReq.Email;
            Request.PhoneNumber = viewPatientReq.Mobile;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.ConfirmationNumber = viewPatientReq.City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + viewPatientReq.LastName.Substring(0, 2) + viewPatientReq.FirstName.Substring(0, 2) + "002";

            _context.Requests.Add(Request);
             _context.SaveChanges();    
            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewPatientReq.FirstName;
            Requestclient.LastName = viewPatientReq.LastName;
            Requestclient.Address = viewPatientReq.Street + "," + viewPatientReq.City + "," + Enum.GetName(typeof(RegionList), viewPatientReq.State) + "," + viewPatientReq.ZipCode;
            Requestclient.Email = viewPatientReq.Email;
            Requestclient.PhoneNumber = viewPatientReq.Mobile;
            Requestclient.Notes = viewPatientReq.Symptoms;
            Requestclient.IntDate = viewPatientReq.DOB.Day;
            Requestclient.IntYear = viewPatientReq.DOB.Year;
            Requestclient.Street = viewPatientReq.Street;
            Requestclient.City = viewPatientReq.City;
            Requestclient.State = Enum.GetName(typeof(RegionList),viewPatientReq.State);
            Requestclient.RegionId = viewPatientReq.State;
            Requestclient.StrMonth = (viewPatientReq.DOB.Month).ToString();
            Requestclient.ZipCode = viewPatientReq.ZipCode;
            Requestclient.CreatedDate = DateTime.Now;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewPatientReq.file != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, viewPatientReq.file.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewPatientReq.file.CopyTo(stream);
                }

                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewPatientReq.file.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1),
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
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewFamilyReq.Email);
           
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:7151/Home/Register?Email=" + viewFamilyReq.Email;
                var template = $"<a href='{agreementUrl}'>Create Account</a>";
                var sent = _emailConfig.SendMail(viewFamilyReq.Email, Subject,template).Result;
                EmailLog em = new EmailLog
                {
                    
                    EmailTemplate = template,
                    SubjectName = Subject,
                    EmailId = viewFamilyReq.Email,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 6,// action 6 for registration
                    RoleId = 1,// role 1 for patient
                };

                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
            }
            Guid g = Guid.NewGuid();


            Request.RequestTypeId = 3;
            Request.Status = 1;
            if (isexist != null)
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
            Request.ConfirmationNumber = viewFamilyReq.City.Substring(0, 2) + DateTime.Now.ToString("dMM") + viewFamilyReq.LastName.Substring(0, 2) + viewFamilyReq.FirstName.Substring(0, 2) + "002";
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = viewFamilyReq.FirstName;
            Requestclient.LastName = viewFamilyReq.LastName;
            Requestclient.Address = viewFamilyReq.Street + "," + viewFamilyReq.City + "," + Enum.GetName(typeof(RegionList), viewFamilyReq.State) + "," + viewFamilyReq.ZipCode;
            Requestclient.Email = viewFamilyReq.Email;
            Requestclient.PhoneNumber = viewFamilyReq.Mobile;
            Requestclient.Notes = viewFamilyReq.Symptoms;
            Requestclient.IntDate = viewFamilyReq.DOB.Day;
            Requestclient.IntYear = viewFamilyReq.DOB.Year;
            Requestclient.Street = viewFamilyReq.Street;
            Requestclient.City = viewFamilyReq.City;
            Requestclient.State = Enum.GetName(typeof(RegionList), viewFamilyReq.State);
            Requestclient.RegionId = viewFamilyReq.State;
            Requestclient.StrMonth = (viewFamilyReq.DOB.Month).ToString();
            Requestclient.ZipCode = viewFamilyReq.ZipCode;
            Requestclient.CreatedDate = DateTime.Now;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewFamilyReq.file != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, viewFamilyReq.file.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewFamilyReq.file.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewFamilyReq.file.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1),
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
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewBusinessReq.Email);
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:7151/Home/Register?Email=" + viewBusinessReq.Email;
                var template = $"<a href='{agreementUrl}'>Create Account</a>";
                var sent = _emailConfig.SendMail(viewBusinessReq.Email, Subject, template).Result;
                EmailLog em = new EmailLog
                {

                    EmailTemplate = template,
                    SubjectName = Subject,
                    EmailId = viewBusinessReq.Email,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 6,// action 6 for registration
                    RoleId = 1,// role 1 for patient
                };

                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
            }

            Request.RequestTypeId = 1;
            Request.Status = 1;
            if (isexist != null)
            {
                Request.UserId = isexist.UserId;
            }
            Request.FirstName = viewBusinessReq.First_name;
            Request.LastName = viewBusinessReq.Last_name;
            Request.Email = viewBusinessReq.Emailid;
            Request.PhoneNumber = viewBusinessReq.Mobileno;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.ConfirmationNumber = viewBusinessReq.City.Substring(0, 2) + DateTime.Now.ToString("dMM") + viewBusinessReq.LastName.Substring(0, 2) + viewBusinessReq.FirstName.Substring(0, 2) + "002";
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
            Requestclient.Street = viewBusinessReq.Street;
            Requestclient.City = viewBusinessReq.City;
            Requestclient.State = Enum.GetName(typeof(RegionList), viewBusinessReq.State);
            Requestclient.RegionId = viewBusinessReq.State;
            Requestclient.StrMonth = (viewBusinessReq.DOB.Month).ToString();
            Requestclient.ZipCode = viewBusinessReq.ZipCode;
            Requestclient.CreatedDate = DateTime.Now;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            Business.Name = viewBusinessReq.First_name + " " + viewBusinessReq.Last_name;
            Business.Address1 = viewBusinessReq.Businessname;
            Business.CreatedDate = DateTime.Now;
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
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewConciergeReq.Email);
            Guid g = Guid.NewGuid();
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:7151/Home/Register?Email=" +  viewConciergeReq.Email;
                var template = $"<a href='{agreementUrl}'>Create Account</a>";
                var sent = _emailConfig.SendMail(viewConciergeReq.Email, Subject, template).Result;
                EmailLog em = new EmailLog
                {

                    EmailTemplate = template,
                    SubjectName = Subject,
                    EmailId = viewConciergeReq.Email,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1),
                    SentTries = 1,
                    Action = 6,// action 6 for registration
                    RoleId = 1,// role 1 for patient
                };

                if (sent)
                {
                    em.IsEmailSent[0] = true;
                };
                _context.EmailLogs.Add(em);
                _context.SaveChanges();
            }


            Request.RequestTypeId = 4;
            Request.Status = 1;
            if (isexist != null)
            {
                Request.UserId = isexist.UserId;
            }
            
            Request.FirstName = viewConciergeReq.First_name;
            Request.LastName = viewConciergeReq.Last_name;
            Request.Email = viewConciergeReq.Emailid;
            Request.PhoneNumber = viewConciergeReq.Mobileno;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.ConfirmationNumber = viewConciergeReq.City.Substring(0, 2) + DateTime.Now.ToString("dMM") + viewConciergeReq.LastName.Substring(0, 2) + viewConciergeReq.FirstName.Substring(0, 2) + "002";
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
            Requestclient.Street = viewConciergeReq.Street;
            Requestclient.City = viewConciergeReq.City;
            Requestclient.State = Enum.GetName(typeof(RegionList), viewConciergeReq.State);
            Requestclient.RegionId = viewConciergeReq.State;
            Requestclient.StrMonth = (viewConciergeReq.DOB.Month).ToString();
            Requestclient.ZipCode = viewConciergeReq.ZipCode;
            Requestclient.CreatedDate = DateTime.Now;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            Concierge.ConciergeName = viewConciergeReq.First_name + "" + viewConciergeReq.Last_name;
            Concierge.Street = viewConciergeReq.Street;
            Concierge.City = viewConciergeReq.City;
            Concierge.State = Enum.GetName(typeof(RegionList), viewConciergeReq.State);
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

    
