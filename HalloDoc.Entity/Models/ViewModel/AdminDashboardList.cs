﻿using HalloDoc.Entity.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class AdminList
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; }
        public string ProviderName { get; set; }
        public DateTime DOB { get; set; }
        public string PhoneNumber { get; set; }
        public int RequestTypeId { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string RequestorPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
        public int RequestClientId { get; set; }
        public int Status { get; set; }
       
    }
    public class PaginatedViewModel<T>
    {
        public List<T> AdminList { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
        public int Count { get; set; }
        public string sortColumn { get; set; } = "RequestedDate";
        public bool sortOrder { get; set; } = false;
    }

    public class CountStatusWiseRequestModel
    {
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
          
    }
    public class sendAgreement
    {
        public int RequestId { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }
    public class AdminProfile
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"(^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$)", ErrorMessage = "Please enter valid Email Address.")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "ConfirmEmail is required")]
        [Compare("Email", ErrorMessage = "Email doesn't match.")]
        public string ConformEmail { get; set; }
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
       
        public string? City { get; set; }
        [Required(ErrorMessage = "State is required")]
        public int State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string ZipCode { get; set; }
        public List<Region> RegionIds { get; set; }
        public string? RegionIdList { get; set; }
        public int AdminId { get; set; }
    }
}


