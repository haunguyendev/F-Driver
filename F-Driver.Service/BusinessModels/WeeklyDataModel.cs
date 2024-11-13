using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class WeeklyDataModel
    {
        public int Week { get; set; }
        public int UserCount { get; set; }
        public int TripRequestCount { get; set; }
    }
}
