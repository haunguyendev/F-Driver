using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.Shared;
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

            // Kiểm tra quyền của Driver
            if (payment.DriverId != driverId)
            {
                throw new UnauthorizedAccessException("You do not have permission to confirm this payment.");
            }

            // Kiểm tra trạng thái Payment
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
    }
}
