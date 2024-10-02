﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class TripMatch : EntityBase
{

    public int? TripRequestId { get; set; }  // Chuyến đi liên quan đến ghép cặp này

    public int? DriverId { get; set; }       // Tài xế nào đang ghép cặp

    public int? PassengerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? MatchedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [InverseProperty("TripMatch")]
    public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();


    [InverseProperty("Match")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Match")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    [ForeignKey("DriverId")]
    [InverseProperty("TripMatches")]
    public virtual Driver? Driver { get; set; }

    // Quan hệ với Passenger (User)
    [ForeignKey("PassengerId")]
    [InverseProperty("TripMatchesAsPassenger")]
    public virtual User? Passenger { get; set; }
    [ForeignKey("TripRequestId")]
    [InverseProperty("TripMatches")]
    public virtual TripRequest? TripRequest { get; set; }

    [InverseProperty("Match")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
