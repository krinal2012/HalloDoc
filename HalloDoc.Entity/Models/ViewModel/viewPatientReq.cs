using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class viewPatientReq
    {
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "Symptoms is required")]
        public   string Symptoms { get; set; }
        [Required(ErrorMessage = "First name is required")]      
        public required string FirstName { get; set; }       
        public string? LastName { get; set; }       
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public required string Email { get; set; }
        public string Password { get; set; }
        public string Pass { get; set; }
        [Compare("Pass", ErrorMessage = "Password doesn't match.")]
        public string ConformPass { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string ZipCode { get; set; }
        public string? Room { get; set; }
        public IFormFile? file { get; set; }
        public int RequestId { get; set; }     

    }
    public class viewFamilyReq
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string First_name { get; set; }
        public string? Last_name { get; set; }
        public string? Mobileno { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string Emailid { get; set; }
        public string? Relation { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string ZipCode { get; set; }
        public string? Room { get; set; }
        public IFormFile? file { get; set; }
    }
    public class viewBusinessReq
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public required string Symptoms { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public required string First_name { get; set; }
        public string? Last_name { get; set; }
        public string? Mobileno { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public required string Emailid { get; set; }
        public string? Hotelname { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public required string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public required string ZipCode { get; set; }
        public string? Room { get; set; }
        public string? Businessname { get; set; }
        public string? Caseno { get; set; }

    }
    public class viewConciergeReq
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string First_name { get; set; }
        public string? Last_name { get; set; }
        public string? Mobileno { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string Emailid { get; set; }
        public string? Hotelname { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string ZipCode { get; set; }
        public string? Room { get; set; }
    }
}



