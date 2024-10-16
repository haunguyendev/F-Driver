using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class WalletModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public decimal? Balance { get; set; }
       public DateTime? CreatedAt { get; set; }
    }
}
