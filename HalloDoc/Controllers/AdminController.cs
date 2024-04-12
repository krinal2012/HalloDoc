using AspNetCoreHero.ToastNotification.Abstractions;
using Hallodoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Models;
using HalloDoc.Repository.Repository;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Cryptography;
using System.Web.Helpers;
using static HalloDoc.Entity.Models.Constant;
using static HalloDoc.Repository.Repository.JWTService;

namespace HalloDoc.Controllers
{
    //  [CheckProviderAccess("Admin")]
    [CustomAuthorize("Admin")]
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
        //[CheckAdminAccess]
        public IActionResult Index()
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            ViewBag.CaseReason = _IAdminDash.CaseReason();
            CountStatusWiseRequestModel count = _IAdminDash.CountRequestData();
            return View(count);
        }
        public IActionResult GetPartialView(string btnName, int statusid, string searchValue, string sortColumn, string sortOrder, int pagesize = 5, int requesttype = -1, int Region = -1, int page = 1)
        {
            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", statusid.ToString());
            Response.Cookies.Append("StatusName", btnName);
            var partialview = "_" + btnName;
            var result = _IAdminDash.NewRequestData(statusid, searchValue, page, pagesize, Region, sortColumn, sortOrder, requesttype);
            return PartialView(partialview, result);
        }
        public IActionResult viewCase(int RequestId, int RequestTypeId, int status)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var result = _IAdminDash.ViewCaseData(RequestId, RequestTypeId, status);
            return View(result);
        }
        [HttpPost]
        public IActionResult viewCase(int RequestId, int RequestTypeId, ViewCaseModel vp)
        {
            ViewBag.AssignCase = _IAdminDash.AssignCase();
            var result = _IAdminDash.EditViewCaseData(RequestId, RequestTypeId, vp);
            return View(result);
        }
        public IActionResult viewNotes(int RequestId)
        {
            viewNotesData result = _IAdminDash.viewNotesData(RequestId);
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
        public async Task<IActionResult> CloseCase(int RequestId)
        {
            ViewCaseModel vc = _IAdminDash.CloseCaseData(RequestId);
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
            return View(ei);
        }
        [HttpPost]
        public IActionResult EncounterForm(ViewEncounterForm ve)
        {
            _IAdminDash.EditEncounterinfo(ve);
            return View();
        }
        public IActionResult EncounterFinalize(ViewEncounterForm ve)
        {
            bool result = _IAdminDash.Finalizeform(ve);
            return RedirectToAction("Index", "Admin");
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
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
            var UserId = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;

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
    }
}


