using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Entity.DataModels;

[Table("Timesheet")]
public partial class Timesheet
{
    [Key]
    public int TimesheetId { get; set; }

    public int? PhysicianId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? EndDate { get; set; }

    public bool IsFinalized { get; set; }

    public bool? IsApproved { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("Timesheets")]
    public virtual Physician? Physician { get; set; }
}
