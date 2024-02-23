using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Entity.DataModels;

[Table("RequestClient")]
public partial class RequestClient
{
    [Key]
    public int RequestClientId { get; set; }

    public int RequestId { get; set; }

    [StringLength(128)]
    public string FirstName { get; set; } = null!;

    [StringLength(128)]
    public string? LastName { get; set; }

    [StringLength(28)]
    public string? PhoneNumber { get; set; }

    [StringLength(128)]
    public string? Location { get; set; }

    [StringLength(528)]
    public string? Address { get; set; }

    [Column("RegionId ")]
    public int? RegionId { get; set; }

    [Column("NotiMobile ")]
    [StringLength(20)]
    public string? NotiMobile { get; set; }

    [Column("NotiEmail ")]
    [StringLength(56)]
    public string? NotiEmail { get; set; }

    [StringLength(528)]
    public string? Notes { get; set; }

    [StringLength(56)]
    public string? Email { get; set; }

    [Column("strMonth")]
    [StringLength(20)]
    public string? StrMonth { get; set; }

    [Column("intYear")]
    public int? IntYear { get; set; }

    [Column("intDate")]
    public int? IntDate { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsMobile { get; set; }

    [StringLength(128)]
    public string? Street { get; set; }

    [Column("City ")]
    [StringLength(128)]
    public string? City { get; set; }

    [StringLength(128)]
    public string? State { get; set; }

    [Column("ZipCode ")]
    [StringLength(20)]
    public string? ZipCode { get; set; }

    public short? CommunicationType { get; set; }

    public short? RemindReservationCount { get; set; }

    [Column("RemindHouseCallCount ")]
    public short? RemindHouseCallCount { get; set; }

    [Column("IsSetFollowupSent ")]
    public short? IsSetFollowupSent { get; set; }

    [Column("IP ")]
    [StringLength(20)]
    public string? Ip { get; set; }

    public short? IsReservationReminderSent { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("RequestClients")]
    public virtual Region? Region { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("RequestClients")]
    public virtual Request Request { get; set; } = null!;
}
