using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class Transaction : EntityBase
{
    

    public int? WalletId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? TransactionDate { get; set; }

    [ForeignKey("WalletId")]
    [InverseProperty("Transactions")]
    public virtual Wallet? Wallet { get; set; }
}
