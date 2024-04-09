using HalloDoc.Entity.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class PhysiciansData
    {
        public int? notificationid { get; set; }
        public bool IsNotificationStopped { get; set; }
        public string? Role { get; set; }
        public int? onCallStatus { get; set; } = 0;
        public int? shiftid { get; set; }
        public int Physicianid { get; set; }
        public string? Aspnetuserid { get; set; }
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public string? RegionIdList { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string ConformEmail { get; set; } = null!;
        public string? Mobile { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Medicallicense { get; set; }
        public string? Photo { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public string? AdminNotes { get; set; }
        public bool Isagreementdoc { get; set; }
        public bool Isbackgrounddoc { get; set; }
        public bool Istrainingdoc { get; set; }
        public bool? IsNonDisclosureDoc { get; set; }
        public bool Islicensedoc { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public int? Regionid { get; set; }
        public string? Altphone { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public state? Status { get; set; }
        public string BusinessName { get; set; } = null!;
        public string BusinessWebsite { get; set; } = null!;
        public BitArray? Isdeleted { get; set; }
        public int? Roleid { get; set; }
        public string? NpiNumber { get; set; }
        public string? Signature { get; set; }
        public IFormFile? SignatureFile { get; set; }
        public bool Iscredentialdoc { get; set; }
        public BitArray? Istokengenerate { get; set; }
        public string? SyncEmailaddress { get; set; }
        public IFormFile? Agreementdoc { get; set; }
        public IFormFile? NonDisclosuredoc { get; set; }
        public IFormFile? Trainingdoc { get; set; }
        public IFormFile? BackGrounddoc { get; set; }
        public IFormFile? Licensedoc { get; set; }
        public List<Region>? RegionIds { get; set; }
        //public class Regions
        //{
        //    public int? regionid { get; set; }
        //    public string? regionname { get; set; }
        //}
        
    }
}
