using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class Zone : EntityBase
{
    

    [StringLength(255)]
    
    public string ZoneName { get; set; } = null!;

    [StringLength(255)]

    public string? Description { get; set; }

    [InverseProperty("FromZone")]
    public virtual ICollection<PriceTable> PriceTableFromZones { get; set; } = new List<PriceTable>();

    [InverseProperty("ToZone")]
    public virtual ICollection<PriceTable> PriceTableToZones { get; set; } = new List<PriceTable>();

    [InverseProperty("FromZone")]
    public virtual ICollection<TripRequest> TripRequestFromZones { get; set; } = new List<TripRequest>();

    [InverseProperty("ToZone")]
    public virtual ICollection<TripRequest> TripRequestToZones { get; set; } = new List<TripRequest>();
}
