using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class ZoneRequestModel
    {
        [Required]
        public string ZoneName { get; set; } = null!;
        public string? Description { get; set; }

        public ZoneModel MapToZoneModel()
        {
            return new ZoneModel
            {
                ZoneName = ZoneName,
                Description = Description
            };
        }
    }
}
