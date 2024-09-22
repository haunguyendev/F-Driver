using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

[Table("PriceTable")]
public partial class PriceTable : EntityBase
{
   

    public int FromZoneId { get; set; }

    public int ToZoneId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("FromZoneId")]
    [InverseProperty("PriceTableFromZones")]
    public virtual Zone FromZone { get; set; } = null!;

    [ForeignKey("ToZoneId")]
    [InverseProperty("PriceTableToZones")]
    public virtual Zone ToZone { get; set; } = null!;
}
