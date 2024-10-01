using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels.QueryParameters
{
    public class UserQueryParameters
    {
        public string? Search { get; set; }                 // Tìm kiếm theo tên, email hoặc số điện thoại
        public string? Email { get; set; }                  // Lọc theo email
        public string? PhoneNumber { get; set; }            // Lọc theo số điện thoại
        public string? Role { get; set; }                   // Lọc theo role
        public string? VerificationStatus { get; set; }     // Lọc theo trạng thái xác thực
        public string? Sort { get; set; } = "Name";         // Sắp xếp theo trường (default: Name)
        public string? SortOrder { get; set; } = "asc";     // Sắp xếp theo thứ tự (default: asc)
        public int Page { get; set; } = 1;                  // Trang (mặc định: 1)
        public int PageSize { get; set; } = 10;
    }
}
