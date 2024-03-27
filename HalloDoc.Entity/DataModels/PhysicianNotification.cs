using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Entity.DataModels;

[Table("PhysicianNotification")]
public partial class PhysicianNotification
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public int PhysicianId { get; set; }

    public bool IsNotificationStopped { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianNotifications")]
    public virtual Physician Physician { get; set; } = null!;
}
