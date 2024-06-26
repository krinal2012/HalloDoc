﻿using HalloDoc.Entity.DataModels;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class PatientDashList
    {       
        public DateTime createdDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ConcludedDate { get; set; }
        public status Status { get; set; }
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public int Fcount { get; set; }
        public string PatientName { get; set; }
        public string Confirmation { get; set; }
        public string Physician { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }
    public class BlockHistory
    {
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? createdDate { get; set; }
        public List<PatientDashList> pd { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
        public string SortColumn { get; set; } = "CreatedDate";
        public string SortOrder { get; set; } = "false";

    }
    public class SearchInputs
    {
        public int ReqStatus { get; set; }
        public string PatientName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RequestTypeID { get; set; }
        public int Role { get; set; }
        public DateTime? StartDOS { get; set; }
        public DateTime? EndDOS { get; set; }
        public string PhyName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public List<SearchRecords> sr { get; set; }
        public List<EmailLogRecords> el { get; set; }
        public List<User> User { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string SortColumn { get; set; } = "CreatedDate";
        public string SortOrder { get; set; } = "false";
    }

    public class SearchRecords
    {
        public string PatientName { get; set; }
        public string Requestor { get; set; }
        public int RequestTypeID { get; set; }
        public int RequestID { get; set; }
        public DateTime? DateOfService { get; set; }
        public DateTime? CloseCaseDate { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? Zip { get; set; }
        public status Status { get; set; }
        public string? Physician { get; set; }
        public string? PhyNotes { get; set; }
        public string? CancelByPhyNotes { get; set; }
        public string? AdminNotes { get; set; }
        public string? PatientNotes { get; set; }
        public DateTime? Modifieddate { get; set; }
    }

}
