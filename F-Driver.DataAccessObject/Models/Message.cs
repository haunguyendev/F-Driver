using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class Message : EntityBase
{
    

    public int? MatchId { get; set; }

    public int? SenderId { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? SentAt { get; set; }

    [ForeignKey("MatchId")]
    [InverseProperty("Messages")]
    public virtual TripMatch? Match { get; set; }

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User? Sender { get; set; }
}
