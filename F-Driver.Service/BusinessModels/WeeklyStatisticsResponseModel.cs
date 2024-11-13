using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class WeeklyStatisticsResponseModel
    {
        public List<WeeklyDataModel> WeeklyData { get; set; } = new List<WeeklyDataModel>();
    }
}
