using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class DashboardResponseModel
    {
        // Thông tin tổng quan người dùng
        public int TotalUsers { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalPassengers { get; set; }
        public int NewUsersThisMonth { get; set; }

        // Thông tin tổng quan chuyến đi
        public int TotalTripRequests { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelTrips { get; set; }
        public int PendingTrips { get; set; }
        public double CompletionRate { get; set; } // % hoàn thành chuyến đi

        // Thông tin chi tiết yêu cầu ghép chuyến
        public int TotalTripMatches { get; set; }
        public int PendingTripMatches { get; set; }
        public int AcceptedTripMatches { get; set; }
        public int RejectedTripMatches { get; set; }
        public int CanceledTripMatches { get; set; }
        public int InProgressTripMatches { get; set; }
        public int CompletedTripMatches { get; set; }

        // Thông tin về tài chính
        public decimal TotalTransactionAmount { get; set; }

        // Thông tin về khu vực (Zone)
        public int TotalZones { get; set; }
    }

}
