using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class ZoneModel
    {
        public int Id { get; set; }
        public string ZoneName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
