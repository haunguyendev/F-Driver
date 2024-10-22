using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels.QueryParameters
{
    public class PriceTableQueryParams
    {
        public int? FromZoneId { get; set; }
        public int? ToZoneId { get; set; }
        public decimal? MinUnitPrice { get; set; }
        public decimal? MaxUnitPrice { get; set; }
        public string? Sort { get; set; } = "FromZoneId";  // Trường mặc định để sắp xếp
        public string? SortOrder { get; set; } = "asc";    // Thứ tự mặc định là tăng dần
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
