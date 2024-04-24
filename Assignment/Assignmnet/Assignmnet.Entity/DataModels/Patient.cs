using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.DataModels;

[Table("Patient")]
public partial class Patient
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string FirstName { get; set; } = null!;

    [StringLength(128)]
    public string? LastName { get; set; }

    public int? DoctortId { get; set; }

    public int? Age { get; set; }

    [Column(TypeName = "character varying")]
    public string? Email { get; set; }

    [Column(TypeName = "character varying")]
    public string? PhoneNo { get; set; }

    [Column(TypeName = "character varying")]
    public string? Gender { get; set; }

    [Column(TypeName = "character varying")]
    public string? Disease { get; set; }

    [Column(TypeName = "character varying")]
    public string? Specialist { get; set; }

    [ForeignKey("DoctortId")]
    [InverseProperty("Patients")]
    public virtual Doctor? Doctort { get; set; }
}
