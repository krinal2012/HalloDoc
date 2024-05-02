using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Entity.DataModels;

[Table("TimesheetReciept")]
public partial class TimesheetReciept
{
    [Key]
    public int TimesheetRecieptId { get; set; }

    public int TimesheetDetailsId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime Date { get; set; }

    [Column(TypeName = "character varying")]
    public string? Item { get; set; }

    public int? Amount { get; set; }

    [Column(TypeName = "character varying")]
    public string? BillName { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }
}
