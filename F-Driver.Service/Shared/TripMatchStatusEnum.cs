using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Shared
{
    public static class TripMatchStatusEnum
    {
        public const string Pending = "Pending";         // Khi driver vừa gửi yêu cầu ghép cặp
        public const string Accepted = "Accepted";       // Khi passenger chấp nhận yêu cầu ghép cặp của driver
        public const string Canceled = "Canceled";       // Khi passenger hoặc driver hủy yêu cầu ghép cặp
        public const string InProgress = "InProgress";   // Khi driver bắt đầu chuyến đi
        public const string Completed = "Completed";     // Khi chuyến đi đã hoàn thành
        public const string Rejected = "Rejected";
    }


}
