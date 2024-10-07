using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class TripMatch : EntityBase
{

    public int? TripRequestId { get; set; }  // Chuyến đi liên quan đến ghép cặp này

    public int? DriverId { get; set; }       // Tài xế nào đang ghép cặp


    [Column(TypeName = "datetime")]
    public DateTime? MatchedAt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? StartedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [InverseProperty("TripMatch")]
    public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();


    [InverseProperty("Match")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Match")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    // Quan hệ với Passenger (User)
    [ForeignKey("DriverId")]
    [InverseProperty("TripMatchesAsDriver")]
    public virtual User? Driver { get; set; }
    [ForeignKey("TripRequestId")]
    [InverseProperty("TripMatches")]
    public virtual TripRequest? TripRequest { get; set; }

    [InverseProperty("Match")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
