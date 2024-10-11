using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class FeedbackCreateModel
    {
        public int MatchId { get; set; }
        public int DriverId { get; set; }
        public int PassengerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
