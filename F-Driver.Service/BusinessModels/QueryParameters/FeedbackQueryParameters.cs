using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels.QueryParameters
{
    public class FeedbackQueryParameters
    {
        public int? Rating { get; set; }                    // Lọc theo đánh giá
        public string? Sort { get; set; } = "CreatedAt";    // Sắp xếp theo trường (default: CreatedAt)
        public string? SortOrder { get; set; } = "desc";    // Sắp xếp theo thứ tự (default: desc)
        public int Page { get; set; } = 1;                  // Trang (mặc định: 1)
        public int PageSize { get; set; } = 10;             // Số lượng phần tử trên mỗi trang (mặc định: 10)
    }
}
