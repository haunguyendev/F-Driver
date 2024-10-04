using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels.QueryParameters
{
    public class ZoneQueryParameters
    {
        public string? ZoneName { get; set; } // Search or filter by zone name
        public string? Description { get; set; } // Search or filter by description

        // Sorting properties
        public string SortBy { get; set; } = "ZoneName"; // Default sorting field
        public bool IsAscending { get; set; } = true;

        // Paging properties
        public int Page { get; set; } = 1;                  // Trang (mặc định: 1)
        public int PageSize { get; set; } = 10;
    }
}
