﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class TripRequest : EntityBase
{
   

    public int? UserId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    public int FromZoneId { get; set; }

    public int ToZoneId { get; set; }

    public DateOnly TripDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public int? Slot { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("FromZoneId")]
    [InverseProperty("TripRequestFromZones")]
    public virtual Zone FromZone { get; set; } = null!;

    [ForeignKey("ToZoneId")]
    [InverseProperty("TripRequestToZones")]
    public virtual Zone ToZone { get; set; } = null!;

    [InverseProperty("DriverRequest")]
    public virtual ICollection<TripMatch> TripMatchDriverRequests { get; set; } = new List<TripMatch>();

    [InverseProperty("PassengerRequest")]
    public virtual ICollection<TripMatch> TripMatchPassengerRequests { get; set; } = new List<TripMatch>();

    [ForeignKey("UserId")]
    [InverseProperty("TripRequests")]
    public virtual User? User { get; set; }
}
