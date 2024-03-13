using System.Collections;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class viewProfile
    { 
            public int? UserId { get; set; }
            public string? Aspnetuserid { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string Email { get; set; }
            public string? Mobile { get; set; }
            public BitArray? Ismobile { get; set; }
            public string? Street { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public int? Regionid { get; set; }
            public string? Zipcode { get; set; }
            public string? Strmonth { get; set; }
            public DateTime DOB { get; set; }
            public int? Intyear { get; set; }
            public int? Intdate { get; set; }
            public string Createdby { get; set; } = null!;
            public DateTime Createddate { get; set; }
            public string? Modifiedby { get; set; }
            public DateTime? Modifieddate { get; set; }
            public short? Status { get; set; }
            public string? Ip { get; set; }
    }
}
