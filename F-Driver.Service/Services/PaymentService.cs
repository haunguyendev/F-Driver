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

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(payment => new PaymentResponse
                {
                    PaymentId = payment.Id,
                    MatchId = payment.MatchId,
                    PassengerId= payment.Passenger != null ? payment.Passenger.Id : null,
                    PassengerName = payment.Passenger != null ? payment.Passenger.Name : null,
                    DriverId= payment.Driver != null ? payment.Driver.Id : null,
                    DriverName = payment.Driver != null ? payment.Driver.Name : null,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status,
                    PaidAt = payment.PaidAt
                })
                .ToListAsync();

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
