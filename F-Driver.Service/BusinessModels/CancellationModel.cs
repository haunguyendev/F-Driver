using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class CancellationModel
    {
        public int Id { get; set; }

        public int? TripMatchId { get; set; }

        public int? ReasonId { get; set; }

        public DateTime? CanceledAt { get; set; }
    }
}
