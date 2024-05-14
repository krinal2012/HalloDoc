using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Entity.DataModels;

public partial class TimesheetDetail
{
    [Key]
    public int TimesheetDetailsId { get; set; }

    public int TimesheetId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime Date { get; set; }

    public int? OnCallHours { get; set; }

    public int? TotalHours { get; set; }

    public bool? IsWeekend { get; set; }

    public int? NoofHousecall { get; set; }

    public int? NoofPhoneConsult { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CratedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("TimesheetId")]
    [InverseProperty("TimesheetDetails")]
    public virtual Timesheet Timesheet { get; set; } = null!;
}
