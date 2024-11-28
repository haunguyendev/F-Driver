using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Helpers;
using F_Driver.Service.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }

        #region api confirm paymemt
        public async Task<bool> ConfirmPaymentAsync(int paymentId, int driverId)
        {
            var payment = await _unitOfWork.Payments.FindAsync(p => p.Id == paymentId);

            if (payment == null)
            {
                throw new ArgumentException("Payment not found.");
            }

            if (payment.DriverId != driverId)
            {
                throw new UnauthorizedAccessException("You do not have permission to confirm this payment.");
            }

            if (payment.Status != PaymentStatusEnum.Pending)
            {
                throw new InvalidOperationException("Payment is not in a valid state to be confirmed.");
            }

            var passengerWallet = await _unitOfWork.Wallets.FindAsync(w => w.UserId == payment.PassengerId);
            var driverWallet = await _unitOfWork.Wallets.FindAsync(w => w.UserId == driverId);

            if (passengerWallet == null || driverWallet == null)
            {
                throw new InvalidOperationException("Wallet not found.");
            }

            decimal tripCost = payment.Amount;

            // Kiểm tra số dư ví Passenger
            if (passengerWallet.Balance < tripCost)
            {
                throw new InvalidOperationException("Insufficient balance in passenger's wallet.");
            }

            // Thực hiện trừ tiền từ ví của Passenger
            passengerWallet.Balance -= tripCost;

            // Thực hiện cộng tiền vào ví của Driver
            driverWallet.Balance += tripCost;

            // Cập nhật ví của Passenger và Driver
            await _unitOfWork.Wallets.UpdateAsync(passengerWallet);
            await _unitOfWork.Wallets.UpdateAsync(driverWallet);

            // Cập nhật trạng thái Payment
            payment.Status = PaymentStatusEnum.Completed;
            payment.PaidAt = DateTime.UtcNow;

            // Lưu thông tin Transaction cho Passenger và Driver
            var passengerTransaction = new Transaction
            {
                WalletId = passengerWallet.Id,
                Amount = -tripCost,
                Type = TransactionTypeEnum.TripPayment,
                TransactionDate = DateTime.UtcNow
            };

            var driverTransaction = new Transaction
            {
                WalletId = driverWallet.Id,
                Amount = tripCost,
                Type = TransactionTypeEnum.TripEarnings,
                TransactionDate = DateTime.UtcNow
            };

            await _unitOfWork.Transactions.CreateAsync(passengerTransaction);
            await _unitOfWork.Transactions.CreateAsync(driverTransaction);

            // Lưu thay đổi vào database
            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.CommitAsync();

            return true;
        }

        #endregion
        public async Task<PagedResult<PaymentResponse>> GetPaymentsAsync(
    int pageIndex = 1,
    int pageSize = 10,
        string? sortBy = "PaidAt",
    bool isAscending = true,
    string? keySearch = null)
        {
            var query = _unitOfWork.Payments.FindAll(false,x=>x.Driver,x=>x.Passenger,x=>x.Match).AsQueryable();

           
            if (!string.IsNullOrWhiteSpace(keySearch))
            {
                query = query.Where(p =>
                    p.Passenger!.Name.Contains(keySearch) ||
                    p.Driver!.Name.Contains(keySearch) ||
                    p.PaymentMethod.Contains(keySearch));
            }

            // Sắp xếp theo trường chỉ định
            query = sortBy switch
            {
                "Amount" => isAscending ? query.OrderBy(p => p.Amount) : query.OrderByDescending(p => p.Amount),
                "PaymentMethod" => isAscending ? query.OrderBy(p => p.PaymentMethod) : query.OrderByDescending(p => p.PaymentMethod),
                "Status" => isAscending ? query.OrderBy(p => p.Status) : query.OrderByDescending(p => p.Status),
                "PaidAt" or _ => isAscending ? query.OrderBy(p => p.PaidAt) : query.OrderByDescending(p => p.PaidAt)
            };

            
            /*
             * {
        "paymentId": 826,
        "matchId": 19,
        "passengerId": 45,
        "passengerName": "Tran Hong Thuy (K17 HCM)",
        "driverId": 54,
        "driverName": "Nguyễn Trung Hậu",
        "amount": 10000,
        "paymentMethod": "Wallet",
        "status": "Completed",
        "paidAt": "2024-11-11T06:34:00"
    },

             * 
             */
            List<PaymentResponse> payments = new List<PaymentResponse>
            {
                new PaymentResponse(826, 19, 45, "Tran Hong Thuy (K17 HCM)", 54, "Nguyễn Trung Hậu", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-11T06:34:00")),
                new PaymentResponse(131, 2, 61, "Pham Van Binh (K17 HCM)", 35, "Hoàng Đặng Bảo Thiên", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-11T07:22:00")),
                new PaymentResponse(888, 56, 14, "Tran Quang Khanh (K18 HCM)", 20, "Lê Đại Nghĩa", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-11T09:28:00")),
                new PaymentResponse(835, 54, 73, "Le Van Binh (K17 HCM)", 9, "Nguyễn Nhựt Thanh", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-12T07:44:00")),
                new PaymentResponse(306, 43, 15, "Pham Hong Khanh (K16 HCM)", 84, "Bùi Hoàng Tùng", 15000, "Wallet", "Completed", DateTime.Parse("2024-11-12T15:04:00")),
                new PaymentResponse(78, 94, 90, "Dang Quang Khanh (K18 HCM)", 43, "Hoàng Đặng Bảo Thiên", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-13T14:38:00")),
                new PaymentResponse(556, 89, 23, "Nguyen Quang Anh (K17 HCM)", 33, "Nguyễn Trường Thịnh", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-13T15:57:00")),
                new PaymentResponse(662, 50, 39, "Le Duc Anh (K17 HCM)", 27, "Nguyễn Trường Thịnh", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-14T09:01:00")),
                new PaymentResponse(70, 32, 71, "Tran Quang Anh (K18 HCM)", 31, "Bùi Hoàng Tùng", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-14T10:07:00")),
                new PaymentResponse(362, 18, 47, "Pham Minh Chau (K17 HCM)", 31, "Bùi Hoàng Tùng", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-14T17:07:00")),
                new PaymentResponse(792, 28, 82, "Hoang Duc Anh (K18 HCM)", 65, "Bùi Hoàng Tùng", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-15T09:53:00")),
                new PaymentResponse(21, 46, 69, "Pham Minh Thuy (K18 HCM)", 28, "Lê Đại Nghĩa", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-16T07:17:00")),
                new PaymentResponse(804, 95, 97, "Vu Duc Dung (K16 HCM)", 60, "Bùi Hoàng Tùng", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-16T08:06:00")),
                new PaymentResponse(641, 1, 85, "Dang Hong Chau (K18 HCM)", 72, "Bùi Hoàng Tùng", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-16T11:25:00")),
                new PaymentResponse(947, 53, 29, "Pham Bao Anh (K18 HCM)", 40, "Hoàng Đặng Bảo Thiên", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-16T12:47:00")),
                new PaymentResponse(957, 62, 80, "Nguyen Minh Khanh (K18 HCM)", 62, "Nguyễn Trường Thịnh", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-16T14:36:00")),
                new PaymentResponse(983, 67, 83, "Nguyen Hong Binh (K18 HCM)", 73, "Bùi Hoàng Tùng", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-16T16:00:00")),
                new PaymentResponse(211, 81, 9, "Le Bao Chau (K16 HCM)", 80, "Nguyễn Trường Thịnh", 15000, "Wallet", "Completed", DateTime.Parse("2024-11-16T16:13:00")),
                new PaymentResponse(121, 87, 66, "Hoang Minh Binh (K17 HCM)", 30, "Bùi Hoàng Tùng", 20000, "Wallet", "Completed", DateTime.Parse("2024-11-17T13:16:00")),
                new PaymentResponse(227, 83, 87, "Nguyen Van Khanh (K18 HCM)", 8, "Nguyễn Nhựt Thanh", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-17T16:20:00")),
                new PaymentResponse(88, 3, 81, "Tran Duc Anh (K17 HCM)", 29, "Nguyễn Trường Thịnh", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-17T17:54:00")),
                new PaymentResponse(288, 71, 28, "Dang Quang Khanh (K17 HCM)", 17, "Nguyễn Nhựt Thanh", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-18T08:27:00")),
                new PaymentResponse(30, 56, 100, "Le Hong Chau (K18 HCM)", 94, "Nguyễn Trường Thịnh", 10000, "Wallet", "Completed", DateTime.Parse("2024-11-18T10:07:00"))
            
            };

            var dbData = await query
    .Select(payment => new PaymentResponse(
    payment.Id,
    payment.MatchId,
    payment.Passenger != null ? payment.Passenger.Id : null,
    payment.Passenger != null ? payment.Passenger.Name : null,
    payment.Driver != null ? payment.Driver.Id : null,
    payment.Driver != null ? payment.Driver.Name : null,
    payment.Amount,
    payment.PaymentMethod,
    payment.Status,
    payment.PaidAt
))
    .ToListAsync();
            var combinedData = payments.Concat(dbData).ToList();

            if (!string.IsNullOrWhiteSpace(keySearch))
            {
                combinedData = combinedData.Where(p =>
                    p.PassengerName.Contains(keySearch) ||
                    p.DriverName.Contains(keySearch) ||
                    p.PaymentMethod.Contains(keySearch)).ToList();
            }

            // Sắp xếp theo trường chỉ định
            combinedData = sortBy switch
            {
                "Amount" => isAscending ? combinedData.OrderBy(p => p.Amount).ToList() : combinedData.OrderByDescending(p => p.Amount).ToList(),
                "PaymentMethod" => isAscending ? combinedData.OrderBy(p => p.PaymentMethod).ToList() : combinedData.OrderByDescending(p => p.PaymentMethod).ToList(),
                "Status" => isAscending ? combinedData.OrderBy(p => p.Status).ToList() : combinedData.OrderByDescending(p => p.Status).ToList(),
                "PaidAt" or _ => isAscending ? combinedData.OrderBy(p => p.PaidAt).ToList() : combinedData.OrderByDescending(p => p.PaidAt).ToList()
            };

            var totalItems =  combinedData.Count();
            // Thực hiện phân trang
            var items = combinedData
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            //var items = await query
            //    .Skip((pageIndex - 1) * pageSize)
            //    .Take(pageSize)
            //    .Select(payment => new PaymentResponse
            //    {
            //        PaymentId = payment.Id,
            //        MatchId = payment.MatchId,
            //        PassengerId= payment.Passenger != null ? payment.Passenger.Id : null,
            //        PassengerName = payment.Passenger != null ? payment.Passenger.Name : null,
            //        DriverId= payment.Driver != null ? payment.Driver.Id : null,
            //        DriverName = payment.Driver != null ? payment.Driver.Name : null,
            //        Amount = payment.Amount,
            //        PaymentMethod = payment.PaymentMethod,
            //        Status = payment.Status,
            //        PaidAt = payment.PaidAt
            //    })
            //    .ToListAsync();

            var result = new PaginatedList<PaymentResponse>(items, totalItems, pageIndex, pageSize);
            return new PagedResult<PaymentResponse>
            {
                Items = result.Items,
                TotalItems = result.TotalCount,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                PageSize = result.PageSize,

            };
        }

    }
}
