using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class Payment : EntityBase
{
   

    public int? MatchId { get; set; }

    public int? PassengerId { get; set; }

    public int? DriverId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaidAt { get; set; }

    [ForeignKey("DriverId")]
    [InverseProperty("PaymentsOfDriver")]
    public virtual User? Driver { get; set; }

    [ForeignKey("MatchId")]
    [InverseProperty("Payments")]
    public virtual TripMatch? Match { get; set; }

    [ForeignKey("PassengerId")]
    [InverseProperty("PaymentsOfPassenger")]
    public virtual User? Passenger { get; set; }
}
