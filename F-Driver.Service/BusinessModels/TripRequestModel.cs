using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F_Driver.DataAccessObject.Models;

namespace F_Driver.Service.BusinessModels
{
    public class TripRequestModel
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        public int FromZoneId { get; set; }

        public int ToZoneId { get; set; }
        public string FromZoneName { get; set; }
        public string ToZoneName { get; set; }

        public DateOnly TripDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public int Slot { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
