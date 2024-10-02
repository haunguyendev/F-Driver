using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;


public partial class User : EntityBase
{
   

    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [Column("ProfileImageURL")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ProfileImageUrl { get; set; } 

    [Column("StudentIdCardURL")]
    [StringLength(255)]
    [Unicode(false)]
    public string? StudentIdCardUrl { get; set; } 

    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? StudentId { get; set; }

    public bool? Verified { get; set; }
    public string? VerificationStatus { get; set; }
    public bool? IsMailValid { get; set; }
    public string? DeviceId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual Driver? Driver { get; set; }

    [InverseProperty("Passenger")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Sender")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [InverseProperty("Passenger")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("User")]
    public virtual ICollection<TripRequest> TripRequests { get; set; } = new List<TripRequest>();
    [InverseProperty("Passenger")]
    public virtual ICollection<TripMatch> TripMatchesAsPassenger { get; set; } = new List<TripMatch>();

    [InverseProperty("User")]
    public virtual Wallet? Wallet { get; set; }
}
