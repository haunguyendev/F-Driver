using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class CancellationReason : EntityBase
{
  

    [StringLength(255)]
    [Unicode(false)]
    public string Content { get; set; } = null!;

    [InverseProperty("Reason")]
    public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
}
