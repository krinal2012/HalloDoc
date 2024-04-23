using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Models;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using static HalloDoc.Repository.Repository.JWTService;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Admin,Physician", "Dashboard")]
    public class AdminController : Controller
    {
        private readonly IAdminDash _IAdminDash;
        private readonly HelloDocContext _context;
        private readonly INotyfService _notyf;
        private static IHttpContextAccessor _httpContextAccessor;
        public AdminController(IAdminDash IAdminDash, HelloDocContext context, INotyfService notyf, IHttpContextAccessor httpContextAccessor)
        {
            _IAdminDash = IAdminDash;
            _context = context;
            _notyf = notyf;
            _httpContextAccessor = httpContextAccessor;
        }
       
        [Route("Physician/DashBoard")]
        [Route("Admin/DashBoard")]
        public IActionResult Index()
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.CaseReason = _IAdminDash.CaseReason();
            CountStatusWiseRequestModel count = _IAdminDash.CountRequestData(-1);
            if (Crredntials.Role() == "Physician")
            {
                count = _IAdminDash.CountRequestData(Convert.ToInt32(Crredntials.UserID()));
            }
            return View(count);
        }
        public IActionResult GetPartialView(string btnName, int statusid, string searchValue, string sortColumn, string sortOrder, int pagesize = 5, int requesttype = -1, int Region = -1, int page = 1)
        {
            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", statusid.ToString());
            Response.Cookies.Append("StatusName", btnName);
            var partialview = "_" + btnName;
            var userid = -1;
            if(Crredntials.Role()=="Physician")
            {
                userid = Int32.Parse(Crredntials.UserID());
            }
            var result = _IAdminDash.NewRequestData(userid,statusid, searchValue, page, pagesize, Region, sortColumn, sortOrder, requesttype);
            return PartialView(partialview, result);
        }
        public IActionResult SendLink(string FirstName, string Email)
        {
            sendAgreement sendAgreement = new()
            {
                FirstName = FirstName,
                Email = Email,
            };
            if (_IAdminDash.SendLink(sendAgreement))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult CreateRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateRequest(viewPatientReq viewPatientReq)
        {
            //var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
            //var UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            var UserId = Crredntials.AspNetUserId();
            bool result = _IAdminDash.CreateReq(viewPatientReq, UserId);
            if (result)
            {
                _notyf.Success("Request Created.");
            }
            else
            {
                _notyf.Error("there is some errors...");
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Export(string status)
        {
            var requestData = _IAdminDash.Export(status);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Requestor";
                worksheet.Cells[1, 3].Value = "Request Date";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Notes";
                worksheet.Cells[1, 7].Value = "Physician";
                worksheet.Cells[1, 8].Value = "Birth Date";
                worksheet.Cells[1, 9].Value = "RequestTypeId";
                worksheet.Cells[1, 10].Value = "Email";
                worksheet.Cells[1, 11].Value = "RequestId";

                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = requestData[i].PatientName;
                    worksheet.Cells[i + 2, 2].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 2, 3].Value = requestData[i].RequestedDate;
                    worksheet.Cells[i + 2, 4].Value = requestData[i].PatientPhoneNumber;
                    worksheet.Cells[i + 2, 5].Value = requestData[i].Address;
                    worksheet.Cells[i + 2, 6].Value = requestData[i].Notes;
                    worksheet.Cells[i + 2, 7].Value = requestData[i].ProviderName;
                    worksheet.Cells[i + 2, 8].Value = requestData[i].DOB;
                    worksheet.Cells[i + 2, 9].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 2, 10].Value = requestData[i].Email;
                    worksheet.Cells[i + 2, 11].Value = requestData[i].RequestId;
                }

                byte[] excelBytes = package.GetAsByteArray();

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        public IActionResult RequestSupport(string? Message)
        {
            if (_IAdminDash.SendMessage(Message))
            {
                _notyf.Success("Message Send Successfully..!");
            }
            return RedirectToAction("Index", "Admin");

        }
        public IActionResult viewCase(int RequestId)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var result = _IAdminDash.ViewCaseData(RequestId);
            if(result==null)
            {
                return NotFound();
            }
            return View(result);
        }
        [HttpPost]
        public IActionResult viewCase(int RequestId, int RequestTypeId, ViewCaseModel vp)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var result = _IAdminDash.EditViewCaseData(RequestId, RequestTypeId, vp);
            _notyf.Success("Data Edited Successfully..!");
            return View(result);
        }
        public IActionResult viewNotes(int RequestId)
        {
            viewNotesData result = _IAdminDash.viewNotesData(RequestId);
            if (result == null)
            {
                return NotFound();
            }
            return View("../Admin/viewNotes", result);
            // return View();
        }
        [HttpPost]
        public IActionResult EditNote(int RequestID, string? adminnotes, string? physiciannotes)
        {
            if (adminnotes != null || physiciannotes != null)
            {
                bool result = _IAdminDash.ViewNotes(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    _notyf.Success("Notes Updated successfully...");
                    return RedirectToAction("viewNotes", new { RequestId = RequestID });
                }
                else
                {
                    _notyf.Error("Notes Not Updated");
                    return RedirectToAction("viewNotes", new { RequestId = RequestID });
                }
            }
            else
            {
                _notyf.Error("Error!!");
                return RedirectToAction("viewNotes", new { RequestId = RequestID });
            }
        }
        public IActionResult PhysicianbyRegion(int Regionid)
        {
            var v = _IAdminDash.ProviderbyRegion(Regionid);
            return Json(v);
        }
        public IActionResult AcceptCase(int RequestId, string Notes)
        {
            int phyid = Int32.Parse(Crredntials.UserID());
            if ( _IAdminDash.AcceptCase(RequestId, Notes, phyid))
            {
                _notyf.Success("Case Accepted...");
            }
            else
            {
                _notyf.Success("Case Not Accepted...");
            }
            return Redirect("~/Physician/DashBoard");
        }
        public IActionResult RejectCase(int RequestId, string Notes)
        {
            int phyid = Int32.Parse(Crredntials.UserID());
            if (_IAdminDash.TransferCase(RequestId, Notes, phyid))
            {
                _notyf.Success("Case Rejected...");
            }
            else
            {
                _notyf.Success("Case Not Rejected...");
            }
            return Redirect("~/Physician/DashBoard");
        }
        [HttpPost]
        public IActionResult AssignCase(int RequestId, int PhysicianId, string Notes)
        {
            _IAdminDash.AssignCaseInfo(RequestId, PhysicianId, Notes);
            _notyf.Success("Physician Assigned successfully...");
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult TransferCase(int RequestId, int PhysicianId, string Notes)
        {
            _IAdminDash.TransferCaseInfo(RequestId, PhysicianId, Notes);
            _notyf.Success("Physician Transfered successfully...");
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult CancleCase(int? RequestId, string Notes, string CaseTag)
        {
            bool result = _IAdminDash.CancleCaseInfo(RequestId, Notes, CaseTag);
            if (result)
            {
                _notyf.Success("Case Canceled Successfully");
            }
            else
            {
                _notyf.Error("Case Not Canceled");
            }
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public IActionResult BlockCase(int RequestId, string Notes)
        {
            var res = _IAdminDash.BlockCaseInfo(RequestId, Notes);
            _notyf.Success("Case Blocked Successfully");
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult ViewUploads(int requestid)
        {
            var v = _IAdminDash.ViewUploadsInfo(requestid);
            if (v == null)
            {
                return NotFound();
            }
            return View("../Admin/ViewUploads", v);
        }
        [HttpPost]
        public IActionResult ViewUploads(viewDocument vp, int userid, IFormFile UploadFile)
        {
            var v = _IAdminDash.ViewUploadPost(vp, userid, UploadFile);
            return ViewUploads(vp.RequestId);
        }
        public IActionResult DeleteFile(int id, int requestid)
        {
            _IAdminDash.DeleteFile(id);
            return RedirectToAction("ViewUploads", new { requestid });
        }
        public IActionResult DeleteAllFile(int requestid, int[] files)
        {
            _IAdminDash.Delete(requestid, files);
            return RedirectToAction("ViewUploads", new { requestid });
        }
        public async Task<IActionResult> DocMail(int Reqid, string Email, string mailids)
        {
            if (await _IAdminDash.SendFileEmail(mailids, Reqid, Email))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult SendOrders(int RequestId)
        {
            ViewBag.Professions = _IAdminDash.Professions(RequestId);
            return View();
        }
        public IActionResult VendorByProfession(int Professionid)
        {
            var v = _IAdminDash.VendorByProfession(Professionid);
            return Json(v);
        }
        public IActionResult SendOrdersData(int selectedValue)
        {
            var result = _IAdminDash.SendOrdersInfo(selectedValue);
            return Json(result);
        }
        [HttpPost]
        public IActionResult SendOrders(int Requestid, OrderDetail o, string Notes)
        {
            var v = _IAdminDash.SendOrders(Requestid, o, Notes);
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult ClearCase(int RequestId)
        {
            var v = _IAdminDash.ClearCaseInfo(RequestId);
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult SendAgreementModal(int requestid)
        {
            Entity.DataModels.Request obj = _context.Requests.FirstOrDefault(x => x.RequestId == requestid);
            sendAgreement sendAgreement = new() { RequestId = requestid, PhoneNumber = obj.PhoneNumber, Email = obj.Email };
            return View("_sendAgreement", sendAgreement);
        }
        [HttpPost]
        public IActionResult SendAgreement(int Reqid, string PhoneNumber, string Email)
        {
            sendAgreement sendAgreement = new()
            {
                RequestId = Reqid,
                PhoneNumber = PhoneNumber,
                Email = Email
            };

            if (_IAdminDash.SendAgreement(sendAgreement))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult CloseCase(int RequestId)
        {
            ViewCaseModel vc = _IAdminDash.CloseCaseData(RequestId);
            if (vc == null)
            {
                return NotFound();
            }
            return View("../Admin/CloseCase", vc);
        }
        public IActionResult EditCloseCase(ViewCaseModel vc, int RequestID)
        {
            bool result = _IAdminDash.EditCloseCase(vc, RequestID);
            if (result)
            {
                _notyf.Success("Case Edited Successfully..");
                return RedirectToAction("CloseCase", new { RequestID });
            }
            else
            {
                _notyf.Error("Case Not Edited...");
                return RedirectToAction("CloseCase", new { RequestID });
            }
        }
        public IActionResult CloseCaseUnpaid(int RequestID)
        {
            bool result = _IAdminDash.CloseCase(RequestID);
            if (result)
            {
                _notyf.Success("Case Closed...");
                _notyf.Information("You can see Closed case in unpaid State...");
            }
            else
            {
                _notyf.Error("there is some error in CloseCase...");
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult EncounterForm(int RequestId)
        {
            ViewEncounterForm ei = _IAdminDash.EncounterInfo(RequestId);
            if (ei == null)
            {
                return NotFound();
            }
            return View(ei);
        }
        [HttpPost]
        public IActionResult EncounterForm(ViewEncounterForm ve)
        {
            bool res = _IAdminDash.EditEncounterinfo(ve);
            return View();
        }   
        public IActionResult EncounterFinalize(ViewEncounterForm ve)
        {
            bool save = _IAdminDash.EditEncounterinfo(ve);
            if(save)
            {
                bool result = _IAdminDash.Finalizeform(ve);
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Housecall (int RequestId)
        {
            if (_IAdminDash.Housecall(RequestId))
            {
                _notyf.Success("Case Accepted...");
            }
            else
            {
                _notyf.Error("Case Not Accepted...");
            }
            return Redirect("~/Physician/DashBoard");
        }
        public IActionResult Consult(int RequestId)
        {
            if (_IAdminDash.Consult(RequestId))
            {
                _notyf.Success("Case is in conclude state...");
            }
            else
            {
                _notyf.Error("Error...");
            }
            return Redirect("~/Physician/DashBoard");
        }
        public IActionResult ConcludeCare(int RequestId)
        {
            var v = _IAdminDash.ViewUploadsInfo(RequestId);
            if (v == null)
            {
                return NotFound();
            }
            return View("../Admin/ConcludeCare", v);
        }
        public IActionResult ConcludeCareUploads(viewDocument vp, int userid, IFormFile UploadFile)
        {
            var v = _IAdminDash.ViewUploadPost(vp, userid, UploadFile);
            return ConcludeCare(vp.RequestId);
        }
        public IActionResult ConcludeCareCase(int RequestId, string Notes)
        {
            if(_IAdminDash.ConcludeCare(RequestId, Notes))
            {
                _notyf.Success("Case concluded...");
            }
            else
            {
                _notyf.Error("Please finalize the encounter form first...");
            }
            return Redirect("~/Physician/DashBoard");
        }
        public IActionResult IsEncounterFinalized(int requestId)
        {
            var res = _IAdminDash.IsEncounterFinalized(requestId);
            return Json(res);
        }
        public IActionResult GeneratePdf(int RequestId)
        {
            try
            {
                if (RequestId == 0 || RequestId < 0)
                {
                    throw new Exception("Invalid Request");
                }
                ViewEncounterForm model = _IAdminDash.EncounterInfo(RequestId);
                if (model == null) throw new Exception("Medical Report Not Exist For this Request");
                using (var ms = new MemoryStream())
                {
                    var writer = new PdfWriter(ms)
    ;
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    // Add a title
                    var title = new Paragraph("Medical Report")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20);
                    document.Add(title);
                    // Add a table
                    var table = new Table(new float[] { 4, 6 });
                    table.SetWidth(UnitValue.CreatePercentValue(100));

                    table.AddHeaderCell("Property");
                    table.AddHeaderCell("Value");

                    // Add properties
                    table.AddCell("RequestId");
                    table.AddCell(model.RequestId.ToString());
                    table.AddCell("FirstName");
                    table.AddCell(model.FirstName);
                    table.AddCell("LastName");
                    table.AddCell(model.LastName);
                    table.AddCell("Location");
                    table.AddCell(model.Location ?? "");
                    table.AddCell("DateOfBirth");
                    table.AddCell(model.DOB.ToString());
                    table.AddCell("DateOfService");
                    table.AddCell(model.DOS.ToString());
                    table.AddCell("Mobile");
                    table.AddCell(model.Mobile);
                    table.AddCell("Email");
                    table.AddCell(model.Email);
                    table.AddCell("HistoryOfPresentIllness");
                    table.AddCell(model.Injury ?? "");
                    table.AddCell("MedicalHistory");
                    table.AddCell(model.History ?? "");
                    table.AddCell("Medication");
                    table.AddCell(model.Medications ?? "");
                    table.AddCell("Allergies");
                    table.AddCell(model.Allergies ?? "");
                    table.AddCell("Temprature");
                    table.AddCell(model.Temp ?? "");
                    table.AddCell("HeartRate");
                    table.AddCell(model.HR ?? "");
                    table.AddCell("RespiratoryRate");
                    table.AddCell(model.RR ?? "");
                    table.AddCell("BloodPressureDiastolic");
                    table.AddCell(model.Bpd ?? "");
                    table.AddCell("BloodPressureSystolic");
                    table.AddCell(model.Bp ?? "");
                    table.AddCell("O2Level");
                    table.AddCell(model.O2 ?? "");
                    table.AddCell("Pain");
                    table.AddCell(model.Pain ?? "");
                    table.AddCell("HEENT");
                    table.AddCell(model.Heent ?? "");
                    table.AddCell("CvReading");
                    table.AddCell(model.CV ?? "");
                    table.AddCell("Chest");
                    table.AddCell(model.Chest ?? "");
                    table.AddCell("ABD");
                    table.AddCell(model.ABD ?? "");
                    table.AddCell("Extr");
                    table.AddCell(model.Extr ?? "");
                    table.AddCell("Skin");
                    table.AddCell(model.Skin ?? "");
                    table.AddCell("Neuro");
                    table.AddCell(model.Neuro ?? "");
                    table.AddCell("Other");
                    table.AddCell(model.Other ?? "");
                    table.AddCell("Diagnosis");
                    table.AddCell(model.Diagnosis ?? "");
                    table.AddCell("TreatmentPlan");
                    table.AddCell(model.Treatment ?? "");
                    table.AddCell("MedicationDispensed");
                    table.AddCell(model.MDispensed ?? "");
                    table.AddCell("Procedures");
                    table.AddCell(model.Procedures ?? "");
                    table.AddCell("FollowUp");
                    table.AddCell(model.Followup ?? "");
                    document.Add(table);

                    // Close the document
                    document.Close();

                    // Return the PDF as a file
                    byte[] pdfBytes = ms.ToArray();
                    string filename = "Medical-Report-" + RequestId + DateTime.Now.ToString("_dd-MM-yyyy-hh-mm-ss-fff") + ".pdf";
                    return File(pdfBytes, "application/pdf", filename);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
    }
}


