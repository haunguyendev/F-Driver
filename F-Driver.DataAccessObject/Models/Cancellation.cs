using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class Cancellation : EntityBase
{
    public int? TripMatchId { get; set; }

    public int? ReasonId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CanceledAt { get; set; }

    [ForeignKey("ReasonId")]
    [InverseProperty("Cancellations")]
    public virtual CancellationReason? Reason { get; set; }

    [ForeignKey("TripMatchId")]
    [InverseProperty("Cancellations")]
    public virtual TripMatch? TripMatch { get; set; }
}
