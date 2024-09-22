using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

[Index("LicensePlate", Name = "UQ__Vehicles__026BC15CE964ACF2", IsUnique = true)]
public partial class Vehicle : EntityBase
{
   

    public int? DriverId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string LicensePlate { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string VehicleType { get; set; } = null!;

    public bool? IsVerified { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Registration { get; set; } = null!;

    [Column("VehicleImageURL")]
    [StringLength(255)]
    [Unicode(false)]
    public string? VehicleImageUrl { get; set; }

    [Column("RegistrationImageURL")]
    [StringLength(255)]
    [Unicode(false)]
    public string? RegistrationImageUrl { get; set; }

    [ForeignKey("DriverId")]
    [InverseProperty("Vehicles")]
    public virtual Driver? Driver { get; set; }
}
