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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using HalloDoc.Entity.Models;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;


namespace HalloDoc.Repository.Repository
{
    public class AdminDash : IAdminDash
    {
        private readonly HelloDocContext _context;
        private readonly EmailConfiguration _emailConfig;
        public AdminDash(HelloDocContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
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
        public PaginatedViewModel<AdminList> NewRequestData(int statusid, string? searchValue, int page, int pagesize,int? Region, string sortColumn, string sortOrder, int? requesttype)
        {
            List<int> id = new List<int>();
            if (statusid == 1) { id.Add(1); }
            if (statusid == 2) { id.Add(2); }
            if (statusid == 3) id.AddRange(new int[] { 4, 5 });
            if (statusid == 4) { id.Add(6); }
            if (statusid == 5) id.AddRange(new int[] { 3, 7, 8 });
            if (statusid == 6) { id.Add(9); }
        
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
                        where id.Contains(req.Status) && (searchValue == null ||
                               rc.FirstName.Contains(searchValue) || rc.LastName.Contains(searchValue) ||
                               req.FirstName.Contains(searchValue) || req.LastName.Contains(searchValue) ||
                               rc.Email.Contains(searchValue) || rc.PhoneNumber.Contains(searchValue) ||
                               rc.Address.Contains(searchValue) || rc.Notes.Contains(searchValue) ||
                               p.FirstName.Contains(searchValue) || p.LastName.Contains(searchValue) ||
                               rg.Name.Contains(searchValue)) 
                               && (Region == -1 || rc.RegionId == Region) 
                               && (requesttype == -1 || req.RequestTypeId ==requesttype)
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
            switch (sortColumn)
            {
                case "Name":
                    list = sortOrder == "desc" ? list.OrderByDescending(x => x.PatientName).ToList() : list.OrderBy(x => x.PatientName).ToList();
                    break;
                //case "Date":
                //    students = students.OrderBy(s => s.EnrollmentDate);
                //    break;
                //case "date_desc":
                //    students = students.OrderByDescending(s => s.EnrollmentDate);
                //    break;
                //default:
                //    students = students.OrderBy(s => s.LastName);
                //    break;
            }
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<AdminList> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();
           
            PaginatedViewModel<AdminList> viewModel = new PaginatedViewModel<AdminList>()
            {
                AdminList = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };
            return viewModel;
        }
        public ViewCaseModel ViewCaseData(int RequestID, int RequestTypeId, int status)
        {
            ViewCaseModel? list =
                        _context.RequestClients
                       .Where(req => req.Request.RequestId == RequestID)
                       .Select(req => new ViewCaseModel()
                        {
                           Status = status,
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
                            LastName = req.LastName
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
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
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
        public void TransferCaseInfo(int RequestId, int PhysicianId, string Notes)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = request.PhysicianId;
            rsl.TransToPhysicianId = PhysicianId;
            rsl.Notes = Notes;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 2;
            _context.RequestStatusLogs.Add(rsl);
            _context.SaveChanges();

            request.PhysicianId = PhysicianId;
            _context.Requests.Update(request);
            _context.SaveChanges();
        }
        public viewNotesData viewNotesData(int RequestId)
        {
            var req = _context.Requests.FirstOrDefault(W => W.RequestId == RequestId);
            //var symptoms = _context.RequestClients.FirstOrDefault(W => W.RequestId == RequestId);
            var transferlog = (from rs in _context.RequestStatusLogs
                               join py in _context.Physicians on rs.PhysicianId equals py.PhysicianId into pyGroup
                               from py in pyGroup.DefaultIfEmpty()
                               join p in _context.Physicians on rs.TransToPhysicianId equals p.PhysicianId into pGroup
                               from p in pGroup.DefaultIfEmpty()
                               //join a in _context.Admins on rs.AdminId equals a.AdminId into aGroup
                               //from a in aGroup.DefaultIfEmpty()
                               where rs.RequestId == RequestId && rs.Status == 2
                               select new TransferNotesData
                               {
                                   TransPhysician = p.FirstName,
                                   //Admin = a.FirstName,
                                   Physician = py.FirstName,
                                   Requestid = rs.RequestId,
                                   Notes = rs.Notes,
                                   Status = rs.Status,
                                   Physicianid = rs.PhysicianId,
                                   Createddate = rs.CreatedDate,
                                   Requeststatuslogid = rs.RequestStatusLogId,
                                   Transtoadmin = rs.TransToAdmin,
                                   Transtophysicianid = rs.TransToPhysicianId   
                               }).ToList();
            var cancelbyprovider = _context.RequestStatusLogs.Where(E => E.RequestId == RequestId && (E.TransToAdmin != null));
            var cancel = _context.RequestStatusLogs.Where(E => E.RequestId == RequestId && (E.Status == 7 || E.Status == 3));
            var model = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestId);
            viewNotesData allData = new viewNotesData();
            allData.Requestid = RequestId;
            //allData.PatientNotes = symptoms.Notes;
            if (model == null)
            {
                allData.PhysicianNotes = "-";
                allData.AdminNotes = "-";
            }
            else
            {
                allData.Status = req.Status;
                allData.Requestnotesid = model.RequestNotesId;
                allData.PhysicianNotes = model.PhysicianNotes ?? "-";
                allData.AdminNotes = model.AdminNotes ?? "-";
            }

            List<TransferNotesData> transfer = new List<TransferNotesData>();
            foreach (var item in transferlog)
            {
                transfer.Add(new TransferNotesData
                {
                    TransPhysician = item.TransPhysician,
                   // Admin = item.Admin,
                    Physician = item.Physician,
                    Requestid = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.Physicianid,
                    Createddate = item.Createddate,
                    Requeststatuslogid = item.Requeststatuslogid,
                    Transtoadmin = item.Transtoadmin,
                    Transtophysicianid = item.Transtophysicianid
                });
            }
            allData.transfernotes = transfer;
            List<TransferNotesData> cancelbyphysician = new List<TransferNotesData>();
            foreach (var item in cancelbyprovider)
            {
                cancelbyphysician.Add(new TransferNotesData
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancelbyphysician = cancelbyphysician;

            List<TransferNotesData> cancelrq = new List<TransferNotesData>();
            foreach (var item in cancel)
            {
                cancelrq.Add(new TransferNotesData
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancel = cancelrq;
            return allData;
        }
        public bool ViewNotes(string? adminnotes, string? physiciannotes, int RequestID)
        {
            try
            {
                RequestNote notes = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestID);
                if (notes != null)
                {
                    if (physiciannotes != null)
                    {
                        if (notes != null)
                        {
                            notes.PhysicianNotes = physiciannotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (adminnotes != null)
                    {
                        if (notes != null)
                        {
                            notes.AdminNotes = adminnotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    RequestNote rn = new RequestNote
                    {
                        RequestId = RequestID,
                        AdminNotes = adminnotes,
                        PhysicianNotes = physiciannotes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "gg"

                    };
                    _context.RequestNotes.Add(rn);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
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
                    requestData.Status = 3;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog();
                    rsl.RequestId = (int)RequestId;
                    rsl.Notes = Notes;
                    rsl.CreatedDate = DateTime.Now;
                    rsl.Status = 3;
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
            viewDocument items = _context.RequestClients.Include(rc => rc.Request)
                                   .Where(rc => rc.RequestId == requestid)
                                     .Select(rc => new viewDocument()
                                     {
                                         FirstName = rc.FirstName,
                                         LastName = rc.LastName,
                                         ConfirmationNumber = rc.Request.ConfirmationNumber

                                     }).FirstOrDefault();
            List<RequestWiseFile> list = _context.RequestWiseFiles
                      .Where(r => r.RequestId == requestid && r.IsDeleted == new BitArray(1))
                      .OrderByDescending(x => x.CreatedDate)
                      .Select(r => new RequestWiseFile
                      {
                          CreatedDate = r.CreatedDate,
                          FileName = r.FileName,
                          RequestWiseFileId = r.RequestWiseFileId,
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
        public List<HealthProfessionalType> Professions(int RequestId)
        {
            var data = _context.HealthProfessionalTypes.ToList();
            return (data);
        }
        public List<HealthProfessional> VendorByProfession(int Professionid)
        {
            var result = _context.HealthProfessionals
                        .Where(req => req.Profession == Professionid)
                        .ToList();
            return result;
        }
        public HealthProfessional SendOrdersInfo(int selectedValue)
        {
            var result = _context.HealthProfessionals
                        .FirstOrDefault(req => req.VendorId == selectedValue);
            return result;
        }
        public bool SendOrders(int Requestid, OrderDetail data, string Notes)
        {
            //try
            //{
            OrderDetail od = new OrderDetail
            {
                RequestId = Requestid,
                VendorId = data.VendorId,
                FaxNumber = data.FaxNumber,
                Email = data.Email,
                BusinessContact = data.BusinessContact,
                Prescription = Notes,
                NoOfRefill = data.NoOfRefill,
                CreatedDate = DateTime.Now,
            };
            _context.OrderDetails.Add(od);
            _context.SaveChanges();
            return true;
            //}           

            //catch (Exception)
            //{
            //    return false;
            //}

        }
        public bool ClearCaseInfo(int? RequestId)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestId);
                if (requestData != null)
                {
                    requestData.Status = 10;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog();
                    rsl.RequestId = (int)RequestId;
                    rsl.CreatedDate = DateTime.Now;
                    rsl.Status = 10;
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SendAgreement(sendAgreement sendAgreement)
        {
            var agreementUrl = "https://localhost:7151/AgreementPage/Index?RequestID=" + sendAgreement.RequestId;
            _emailConfig.SendMail(sendAgreement.Email, "Agreement for your request", $"Agreement for your request <a href='{agreementUrl}'>Agree/Disagree</a>");
            return true;
        }
        public bool SendAgreement_accept(int RequestID)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;
                rsl.Status = 4;
                rsl.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
            }
            return true;
        }
        public bool SendAgreement_Reject(int RequestID, string Notes)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;
                rsl.Status = 7;
                rsl.Notes = Notes;
                rsl.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
            }
            return true;
        }
        public ViewCaseModel CloseCaseData(int RequestID)
        {
            ViewCaseModel? list =
                       _context.RequestClients
                      .Where(req => req.Request.RequestId == RequestID)
                      .Select(req => new ViewCaseModel()
                      {
                          RequestId = RequestID,
                          ConfNo = req.Address.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                          FirstName = req.FirstName,
                          LastName = req.LastName,
                          DOB = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                          Mobile = req.PhoneNumber,
                          Email = req.Email,
                      }).FirstOrDefault();

            List<RequestWiseFile> list1 = _context.RequestWiseFiles
                     .Where(r => r.RequestId == RequestID && r.IsDeleted == new BitArray(1))
                     .OrderByDescending(x => x.CreatedDate)
                     .Select(r => new RequestWiseFile
                     {
                         CreatedDate = r.CreatedDate,
                         FileName = r.FileName,
                         RequestWiseFileId = r.RequestWiseFileId,
                         RequestId = r.RequestId

                     }).ToList();
            list.documents = list1;
            return list;
        }
        public bool EditCloseCase( ViewCaseModel vp, int RequestID)
        {
            var userToUpdate = _context.RequestClients.FirstOrDefault(x => x.RequestId == RequestID); ;
            if (userToUpdate != null)
            {
                userToUpdate.PhoneNumber = vp.Mobile;
                userToUpdate.Email = vp.Email;
                _context.Update(userToUpdate);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CloseCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 9;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog();
                    rsl.RequestId = RequestID;
                    rsl.CreatedDate = DateTime.Now;
                    rsl.Status = 9;
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public ViewEncounterForm EncounterInfo(int RequestId)
        {
            var encounter = (from rc in _context.RequestClients
                             join en in _context.EncounterForms on rc.RequestId equals en.RequestId into renGroup
                             from subEn in renGroup.DefaultIfEmpty()
                             where rc.RequestId == RequestId
                             select new ViewEncounterForm
                             {
                                 RequestId = rc.RequestId,
                                 FirstName = rc.FirstName,
                                 LastName = rc.LastName,
                                 Location = rc.Address,
                                 DOB = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                 //DOS = (DateTime)subEn.DateOfService,
                                 Mobile = rc.PhoneNumber,
                                 Email = rc.Email,
                                 Injury = subEn.HistoryOfPresentIllnessOrInjury,
                                 History = subEn.MedicalHistory,
                                 Medications = subEn.Medications,
                                 Allergies = subEn.Allergies,
                                 Temp = subEn.Temp,
                                 HR = subEn.Hr,
                                 RR = subEn.Rr,
                                 Bp = subEn.BloodPressureSystolic,
                                 Bpd= subEn.BloodPressureDiastolic,
                                 O2 = subEn.O2,
                                 Pain = subEn.Pain,
                                 Heent = subEn.Heent,
                                 CV = subEn.Cv,
                                 Chest = subEn.Chest,
                                 ABD = subEn.Abd,
                                 Extr = subEn.Extremeties,
                                 Skin = subEn.Skin,
                                 Neuro = subEn.Neuro,
                                 Other = subEn.Other,
                                 Diagnosis = subEn.Diagnosis,
                                 Treatment = subEn.TreatmentPlan,
                                 MDispensed = subEn.MedicationsDispensed,
                                 Procedures = subEn.Procedures,
                                 Followup = subEn.FollowUp
                             }).FirstOrDefault();
            return encounter;
        }
        public void EditEncounterinfo(ViewEncounterForm ve)
        {
            var RC = _context.RequestClients.FirstOrDefault(rc => rc.RequestId == ve.RequestId);
            RC.FirstName = ve.FirstName;
            RC.LastName = ve.LastName;
            RC.Address = ve.Location;
            RC.StrMonth = ve.DOB.Month.ToString();
            RC.IntDate = ve.DOB.Day;
            RC.IntYear = ve.DOB.Year;
            RC.PhoneNumber = ve.Mobile;
            RC.Email = ve.Email;
            _context.Update(RC);

            var E = _context.EncounterForms.FirstOrDefault(e => e.RequestId == ve.RequestId);
            if (E == null)
            {
                E = new EncounterForm { RequestId = (int)ve.RequestId };
                _context.EncounterForms.Add(E);
            }
            E.MedicalHistory = ve.History;
            E.HistoryOfPresentIllnessOrInjury = ve.Injury;
            E.Medications = ve.Medications;
            E.Allergies = ve.Allergies;
            E.Temp = ve.Temp;
            E.Hr = ve.HR;
            E.Rr = ve.RR;
            E.BloodPressureSystolic = ve.Bp;
            E.BloodPressureDiastolic = ve.Bpd;
            E.O2 = ve.O2;
            E.Pain = ve.Pain;
            E.Heent = ve.Heent;
            E.Cv = ve.CV;
            E.Chest = ve.Chest;
            E.Abd = ve.ABD;
            E.Extremeties = ve.Extr;
            E.Skin = ve.Skin;
            E.Neuro = ve.Neuro;
            E.Other = ve.Other;
            E.Diagnosis = ve.Diagnosis;
            E.TreatmentPlan = ve.Treatment;
            E.MedicationsDispensed = ve.MDispensed;
            E.Procedures = ve.Procedures;
            E.FollowUp = ve.Followup;
            E.IsFinalize = false;
            _context.SaveChanges();
        }
        public bool Finalizeform(ViewEncounterForm ve)
        {
            var E = _context.EncounterForms.FirstOrDefault(e => e.RequestId == ve.RequestId);
            E.IsFinalize = true;
            _context.SaveChanges();
            return true;
        }

    }
}

   

