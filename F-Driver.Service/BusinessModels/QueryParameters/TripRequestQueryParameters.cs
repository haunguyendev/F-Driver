using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels.QueryParameters
{
    public class TripRequestQueryParameters
    {
        public int? UserId { get; set; }               // Lọc theo UserId
        public int? FromZoneId { get; set; }           // Lọc theo FromZoneId
        public int? ToZoneId { get; set; }             // Lọc theo ToZoneId
        public DateOnly? TripDate { get; set; }        // Lọc theo TripDate
        public TimeOnly? StartTime { get; set; }       // Lọc theo StartTime
        public string? Status { get; set; }             // Lọc theo Status
        public int Page { get; set; } = 1;              // Trang (mặc định: 1)
        public int PageSize { get; set; } = 10;         // Kích thước trang (mặc định: 10)
    }
}
