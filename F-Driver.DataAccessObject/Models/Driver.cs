using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

[Index("UserId", Name = "UQ__Drivers__1788CC4D29B77C06", IsUnique = true)]
[Index("LicenseNumber", Name = "UQ__Drivers__E8890166980DC78F", IsUnique = true)]
public partial class Driver : EntityBase
{
   

    public int? UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string LicenseNumber { get; set; } = null!;

    public bool? Verified { get; set; }

    [Column("LicenseImageURL")]
    [StringLength(255)]
    [Unicode(false)]
    public string LicenseImageUrl { get; set; } = null!;

    [InverseProperty("Driver")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    

    [ForeignKey("UserId")]
    [InverseProperty("Driver")]
    public virtual User? User { get; set; }

    [InverseProperty("Driver")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
