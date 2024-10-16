using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class TransactionResponseModel
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
