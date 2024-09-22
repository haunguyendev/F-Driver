using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

[Index("UserId", Name = "UQ__Wallets__1788CC4DD4869EDC", IsUnique = true)]
public partial class Wallet : EntityBase
{
    

    public int? UserId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Balance { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Wallet")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    [ForeignKey("UserId")]
    [InverseProperty("Wallet")]
    public virtual User? User { get; set; }
}
