using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class TripMatch : EntityBase
{
   

    public int? DriverRequestId { get; set; }

    public int? PassengerRequestId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? MatchedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [InverseProperty("TripMatch")]
    public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();

    [ForeignKey("DriverRequestId")]
    [InverseProperty("TripMatchDriverRequests")]
    public virtual TripRequest? DriverRequest { get; set; }

    [InverseProperty("Match")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Match")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [ForeignKey("PassengerRequestId")]
    [InverseProperty("TripMatchPassengerRequests")]
    public virtual TripRequest? PassengerRequest { get; set; }

    [InverseProperty("Match")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
