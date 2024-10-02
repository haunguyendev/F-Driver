using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class PriceTableModel
    {
        public int Id { get; set; }

        public int FromZoneId { get; set; }

        public int ToZoneId { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
