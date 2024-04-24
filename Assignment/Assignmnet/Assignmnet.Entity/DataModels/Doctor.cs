using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.DataModels;

[Table("Doctor")]
public partial class Doctor
{
    [Key]
    public int DoctorId { get; set; }

    [StringLength(128)]
    public string? Specialist { get; set; }

    [InverseProperty("Doctort")]
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
