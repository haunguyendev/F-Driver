using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

[Table("Feedback")]
public partial class Feedback : EntityBase
{
    

    public int? MatchId { get; set; }

    public int? PassengerId { get; set; }

    public int? DriverId { get; set; }

    public int? Rating { get; set; }

    [Column(TypeName = "text")]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("DriverId")]
    [InverseProperty("Feedbacks")]
    public virtual Driver? Driver { get; set; }

    [ForeignKey("MatchId")]
    [InverseProperty("Feedbacks")]
    public virtual TripMatch? Match { get; set; }

    [ForeignKey("PassengerId")]
    [InverseProperty("Feedbacks")]
    public virtual User? Passenger { get; set; }
}
