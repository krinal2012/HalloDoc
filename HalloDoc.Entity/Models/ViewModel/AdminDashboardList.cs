using HalloDoc.Entity.DataModels;
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
       
    }
    public class PaginatedViewModel<T>
    {
        public List<T> AdminList { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
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
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public required string Email { get; set; }
        [Compare("Email", ErrorMessage = "Password doesn't match.")]
        public string ConformEmail { get; set; }
        public string Mobile { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string ZipCode { get; set; }
        public List<Region> RegionIds { get; set; }
        public string? RegionIdList { get; set; }
        public int AdminId { get; set; }
    }
}


