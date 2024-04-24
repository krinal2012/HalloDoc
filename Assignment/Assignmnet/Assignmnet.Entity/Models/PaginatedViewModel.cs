using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Entity.Models
{
    public class PaginatedViewModel<T>
    {
        public List<T> List { get; set; }
        public string SearchValue { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public int? Id { get; set; }
        public int? DoctortId { get; set; }
        [Required(ErrorMessage = "Age is required")]
        [RegularExpression(@"([0-9]*$)", ErrorMessage = "It must be of numerics")]
        public int? Age { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"(^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$)", ErrorMessage = "Please enter valid Email Address.")]
        public string? Email { get; set; }

        public string? PhoneNo { get; set; }

        public string? Gender { get; set; }
        [Required(ErrorMessage = "Disease is required")]
        public string? Disease { get; set; }
        [Required(ErrorMessage = "Specialist is required")]
        public string? Specialist { get; set; }

    }

}
