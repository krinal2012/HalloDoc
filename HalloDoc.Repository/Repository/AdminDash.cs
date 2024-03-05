using HalloDoc.Entity.DataContext;
using HalloDoc.Repository.Repository.Interface;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataModels;
using static HalloDoc.Entity.Models.Constant;
using System.Globalization;
using System.Web.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace HalloDoc.Repository.Repository
{
    public class AdminDash : IAdminDash
    {
        private readonly HelloDocContext _context;
        public AdminDash(HelloDocContext context)
        {
            _context = context;
        }
        public CountStatusWiseRequestModel CountRequestData()
        {
            return new CountStatusWiseRequestModel
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => (r.Status == 4 || r.Status == 5)).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => (r.Status == 3 || r.Status == 7 || r.Status == 8)).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count()                
            };
        }
        public List<AdminList> NewRequestData(int statusid, string? searchValue)
        {
            List<int> id=new List<int>();
            if(statusid == 1) { id.Add(1); }
            if (statusid == 2) { id.Add(2); }
            if (statusid == 3)
                id.AddRange(new int[] { 4, 5 });
            if (statusid == 4) { id.Add(6); }
            if (statusid == 5) id.AddRange(new int[] { 3, 7, 8 });
            if (statusid == 6) { id.Add(9); }

            if (searchValue == null)
            {
                var list = (from req in _context.Requests
                            join reqClient in _context.RequestClients
                            on req.RequestId equals reqClient.RequestId into reqClientGroup
                            from rc in reqClientGroup.DefaultIfEmpty()
                            join phys in _context.Physicians
                            on req.PhysicianId equals phys.PhysicianId into physGroup
                            from p in physGroup.DefaultIfEmpty()
                            join reg in _context.Regions
                            on rc.RegionId equals reg.RegionId into RegGroup
                            from rg in RegGroup.DefaultIfEmpty()
                            where id.Contains(req.Status)
                            orderby req.CreatedDate descending
                            select new AdminList
                            {
                                RequestId = req.RequestId,
                                RequestTypeId = req.RequestTypeId,
                                Requestor = req.FirstName + " " + req.LastName,
                                PatientName = rc.FirstName + " " + rc.LastName,
                                DOB = new DateTime((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate),
                                RequestedDate = req.CreatedDate,
                                Email = rc.Email,
                                Region = rg.Name,
                                ProviderName = p.FirstName + " " + p.LastName,
                                PatientPhoneNumber = rc.PhoneNumber,
                                Address = rc.Address,
                                Notes = rc.Notes,
                                RequestClientId= rc.RequestClientId,
                                // ProviderID = req.Physicianid,
                                RequestorPhoneNumber = req.PhoneNumber
                            }).ToList();
                return list;
            }
            else
            {
                var list = (from req in _context.Requests
                            join reqClient in _context.RequestClients
                            on req.RequestId equals reqClient.RequestId into reqClientGroup
                            from rc in reqClientGroup.DefaultIfEmpty()
                            join phys in _context.Physicians
                            on req.PhysicianId equals phys.PhysicianId into physGroup
                            from p in physGroup.DefaultIfEmpty()
                            join reg in _context.Regions
                            on rc.RegionId equals reg.RegionId into RegGroup
                            from rg in RegGroup.DefaultIfEmpty()
                            where id.Contains(req.Status) && rc.FirstName.Contains(searchValue)
                            orderby req.CreatedDate descending
                            select new AdminList
                            {
                                RequestId = req.RequestId,
                                RequestTypeId = req.RequestTypeId,
                                Requestor = req.FirstName + " " + req.LastName,
                                PatientName = rc.FirstName + " " + rc.LastName,
                                DOB = new DateTime((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate),
                                RequestedDate = req.CreatedDate,
                                Email = rc.Email,
                                Region = rg.Name,
                                ProviderName = p.FirstName + " " + p.LastName,
                                PatientPhoneNumber = rc.PhoneNumber,
                                Address = rc.Address,
                                Notes = rc.Notes,
                                RequestClientId = rc.RequestClientId,
                                // ProviderID = req.Physicianid,
                                RequestorPhoneNumber = req.PhoneNumber
                            }).ToList();
                return list;

            }
        }
        public ViewCaseModel ViewCaseData(int RequestID,int RequestTypeId)
        {
            ViewCaseModel? list = 
                        _context.RequestClients
                       .Where(req => req.Request.RequestId== RequestID)
                        .Select(req=> new ViewCaseModel()
                        {
                            RequestId = RequestID,
                            RequestTypeId = RequestTypeId,
                            ConfNo = req.Address.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                            Symptoms = req.Notes,
                            FirstName = req.FirstName,
                            LastName = req.LastName,
                            DOB = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                            Mobile = req.PhoneNumber,
                            Email = req.Email,
                            Address = req.Address
                        }).FirstOrDefault();
            return list;
        }
        public ViewCaseModel EditViewCaseData(int RequestID, int RequestTypeId, ViewCaseModel vp)
        {
            var userToUpdate = _context.RequestClients.FirstOrDefault(x => x.RequestId == RequestID);
            if (userToUpdate != null)
            {                
                userToUpdate.PhoneNumber = vp.Mobile;
                userToUpdate.Email = vp.Email;              
                _context.Update(userToUpdate);
                _context.SaveChanges();
            }
            ViewCaseModel? list =
                        _context.RequestClients
                       .Where(req => req.Request.RequestId == RequestID)
                        .Select(req => new ViewCaseModel()
                        {
                            RequestTypeId = RequestTypeId,
                            ConfNo = req.Address.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                            Symptoms = req.Notes,
                            FirstName = req.FirstName,
                            LastName = req.LastName,
                            DOB = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                            Mobile = req.PhoneNumber,
                            Email = req.Email,
                            Address = req.Address
                        }).FirstOrDefault();
            return list;
        }
        public List<Physician> ProviderbyRegion(int Regionid)
        {
            var result = _context.Physicians
                        .Where(req => req.RegionId == Regionid)
                        .Select(req => new Physician()
                        {
                            PhysicianId = req.PhysicianId,
                            FirstName = req.FirstName,
                            LastName =req.LastName
                        }).ToList();
            return result;

        }
        public List<Region> AssignCase()
        {
            var regiondata = _context.Regions.ToList();
            return (regiondata);
        }
        public void AssignCaseInfo(int RequestId, int PhysicianId, string Notes)
        {
            var request =  _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
            request.PhysicianId = PhysicianId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = PhysicianId;
            rsl.Notes = Notes;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 2;
            _context.RequestStatusLogs.Add(rsl);
            _context.SaveChanges();
            

        }
        public List<CaseTag> CaseReason()
        {
            var data = _context.CaseTags.ToList();
            return (data);
        }
        public bool CancleCaseInfo(int? RequestId, string Notes, string caseTag)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestId);
                if (requestData != null)
                {
                    requestData.CaseTag = caseTag;
                    requestData.Status = 8;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog();
                    rsl.RequestId = (int)RequestId;                   
                    rsl.Notes = Notes;
                    rsl.CreatedDate = DateTime.Now;
                    rsl.Status = 8;
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool BlockCaseInfo(int RequestId, string Notes)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestId);
                if (requestData != null)
                {
                    requestData.Status = 11;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    BlockRequest block = new BlockRequest
                    {
                        RequestId = requestData.RequestId,
                        PhoneNumber = requestData.PhoneNumber,
                        Email = requestData.Email,
                        Reason = Notes,
                        CreatedDate = DateTime.Now,
                    };
                    _context.BlockRequests.Add(block);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
             
            catch (Exception ex)
            {
                return false;
            }
}
        public viewDocument ViewUploadsInfo(int requestid)
        {
            viewDocument items = _context.RequestClients.Include( rc => rc.Request)
                                   .Where( rc => rc.RequestId == requestid)
                                     .Select(rc => new viewDocument()
                                      {                                         
                                          FirstName = rc.FirstName,
                                          LastName = rc.LastName,
                                          ConfirmationNumber = rc.Request.ConfirmationNumber
                                          

                                      }).FirstOrDefault();
            List<RequestWiseFile> list = _context.RequestWiseFiles
                      .Where(r => r.RequestId == requestid && r.IsDeleted == new BitArray(1) )
                      .OrderByDescending(x => x.CreatedDate)
                      .Select(r => new RequestWiseFile
                      {
                          CreatedDate = r.CreatedDate,
                          FileName = r.FileName,
                          RequestWiseFileId =r.RequestWiseFileId,
                          RequestId = r.RequestId

                      }).ToList();
            items.Files = list;
            return items;
        }      

        public bool ViewUploadPost(viewDocument v, int userid, IFormFile UploadFile)
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
                    RequestId = v.RequestId,
                    FileName = UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1),
                   
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public void DeleteFile(int id)
        {
            var userToUpdate = _context.RequestWiseFiles.FirstOrDefault(x => x.RequestWiseFileId == id);
            if (userToUpdate != null)
            {
                userToUpdate.IsDeleted[0] = true;
                _context.Update(userToUpdate);
                _context.SaveChanges();
            }

        }
    }
}
