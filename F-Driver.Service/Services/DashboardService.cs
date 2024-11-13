using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class DashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardResponseModel> GetDashboardSummaryAsync()
        {
            // Tổng số lượng người dùng
            var totalUsers = await _unitOfWork.Users.CountAsync();
            var totalDrivers = await _unitOfWork.Users.CountAsync(x=>x.Role=="Driver");
            var totalPassengers = await _unitOfWork.Users.CountAsync(x => x.Role == "Passenger");

            // Số người dùng mới trong tháng
            var newUsersThisMonth = await _unitOfWork.Users
                .CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-1));

            // Người dùng hoạt động trong 24 giờ qua

            // Thống kê chuyến đi
            var totalTripRequests = await _unitOfWork.TripRequests.CountAsync();
            var completedTrips = await _unitOfWork.TripRequests
                .CountAsync(tr => tr.Status == TripRequestStatusEnum.Completed);
            var failedTrips = await _unitOfWork.TripRequests
                .CountAsync(tr => tr.Status == TripRequestStatusEnum.Canceled);
            var pendingTrips = await _unitOfWork.TripRequests
                .CountAsync(tr => tr.Status == TripRequestStatusEnum.Pending);
            var completionRate = (totalTripRequests > 0)
                ? (double)completedTrips / totalTripRequests * 100
                : 0;

            // Thống kê yêu cầu ghép chuyến
            var totalTripMatches = await _unitOfWork.TripMatches.CountAsync();
            var pendingTripMatches = await _unitOfWork.TripMatches
                .CountAsync(tm => tm.Status == TripMatchStatusEnum.Pending);
            var acceptedTripMatches = await _unitOfWork.TripMatches
                .CountAsync(tm => tm.Status == TripMatchStatusEnum.Accepted);
            var rejectedTripMatches = await _unitOfWork.TripMatches
                .CountAsync(tm => tm.Status == TripMatchStatusEnum.Rejected);
            var canceledTripMatches = await _unitOfWork.TripMatches
                .CountAsync(tm => tm.Status == TripMatchStatusEnum.Canceled);
            var inProgressTripMatches = await _unitOfWork.TripMatches
                .CountAsync(tm => tm.Status == TripMatchStatusEnum.InProgress);
            var completedTripMatches = await _unitOfWork.TripMatches
                .CountAsync(tm => tm.Status == TripMatchStatusEnum.Completed);

            // Thống kê giao dịch
            var totalTransactions = await _unitOfWork.Transactions.CountAsync();

            // Thống kê khu vực (Zone)
            var totalZones = await _unitOfWork.Zones.CountAsync();

            // Thống kê driver

            return new DashboardResponseModel
            {
                TotalUsers = totalUsers,
                TotalDrivers = totalDrivers,
                TotalPassengers = totalPassengers,
                NewUsersThisMonth = newUsersThisMonth,
                TotalTripRequests = totalTripRequests,
                CompletedTrips = completedTrips,
                CancelTrips = failedTrips,
                PendingTrips = pendingTrips,
                CompletionRate = completionRate,
                TotalTripMatches = totalTripMatches,
                PendingTripMatches = pendingTripMatches,
                AcceptedTripMatches = acceptedTripMatches,
                RejectedTripMatches = rejectedTripMatches,
                CanceledTripMatches = canceledTripMatches,
                InProgressTripMatches = inProgressTripMatches,
                CompletedTripMatches = completedTripMatches,
                TotalZones = totalZones,
            };
        }
    }
}
