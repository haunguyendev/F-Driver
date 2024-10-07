using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Shared
{
    public static class TransactionTypeEnum
    {
        public const string TripPayment = "TripPayment";
        public const string TripEarnings = "TripEarnings";
        public const string Refund = "Refund";
        public const string Deposit = "Deposit";
        public const string Withdrawal = "Withdrawal";
    }
}
