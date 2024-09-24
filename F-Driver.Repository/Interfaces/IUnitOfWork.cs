using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        ICancellationReasonRepository CancellationReasons { get; }
        ICancellationRepository Cancellations { get; }
        IDriverRepository Driver { get; }
        IFeedbackRepository Feedbacks { get; }

        IMessageRepository Messages { get; }
        IPaymentRepository Payments { get; }
        IPriceTableRepository PriceTables { get; }
        ITransactionRepository Transactions { get; }
        ITripMatchRepository TripMatchs { get; }
        ITripRequestRepository TripRequests { get; }
        IUserRepository Users { get; }
        IVehicleRepository Vehicles { get; }
        IWalletRepository Wallets { get; }
        IZoneRepository Zones { get; }

        FDriverContext GetDbContext();

        Task<int> CommitAsync();
        Task BeginTransactionAsync();
        Task RollbackAsync();
    }
}
