using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels.QueryParameters
{
    public class TransactionQueryParameters
    {
        public decimal? MinAmount { get; set; }             // Lọc theo số tiền tối thiểu
        public decimal? MaxAmount { get; set; }             // Lọc theo số tiền tối đa
        public string? Type { get; set; }                   // Lọc theo loại giao dịch
        public string? Sort { get; set; } = "TransactionDate"; // Sắp xếp theo trường (default: TransactionDate)
        public string? SortOrder { get; set; } = "desc";    // Sắp xếp theo thứ tự (default: desc)
        public int Page { get; set; } = 1;                  // Trang (mặc định: 1)
        public int PageSize { get; set; } = 10;             // Số lượng phần tử trên mỗi trang (mặc định: 10)
    }
}
