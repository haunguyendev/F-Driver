using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels.QueryParameters
{
    public class TripMatchQueryParameters
    {
        public int? TripRequestId { get; set; }
        public int? DriverId { get; set; }
        public DateTime? MatchedAt { get; set; }
        public string? Status { get; set; }

        // Sorting properties
        public string SortBy { get; set; } = "MatchedAt"; // Default sorting field
        public bool IsAscending { get; set; } = true;

        // Paging properties
        public int Page { get; set; } = 1;                  // Trang (mặc định: 1)
        public int PageSize { get; set; } = 10;
    }
}
