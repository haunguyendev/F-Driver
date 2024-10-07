using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Shared
{
    public static class TripRequestStatusEnum
    {
        public const string Pending = "Pending";        // Khi trip request mới được tạo và chưa có ghép cặp
        public const string Completed = "Completed";    // Khi trip request đã được ghép cặp thành công
        public const string Canceled = "Canceled";      // Khi trip request bị hủy bởi passenger
    }

}
