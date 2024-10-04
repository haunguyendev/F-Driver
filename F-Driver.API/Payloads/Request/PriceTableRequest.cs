using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F_Driver.API.Payloads.Request
{
    public class PriceTableRequest
    {
        [Required]
        public int FromZoneId { get; set; }
        [Required]
        public int ToZoneId { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }

        public PriceTableModel MapToTableModel()
        {
            return new PriceTableModel
            {
                FromZoneId = FromZoneId,
                ToZoneId = ToZoneId,
                UnitPrice = UnitPrice
            };
        }
    }
}
