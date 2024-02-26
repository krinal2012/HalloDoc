using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Hallodoc.Entity.Models.ViewModel
{
    public class viewPatientReq
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "First name is required")]      
        public string FirstName { get; set; }       
        public string? LastName { get; set; }       
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string Email { get; set; }
        public string Pass { get; set; }
        [CompareAttribute("Pass", ErrorMessage = "Password doesn't match.")]
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
}
