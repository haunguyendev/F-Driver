using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FDriverContext _context;
        private IDbContextTransaction _currentTransaction;

        public ICancellationReasonRepository CancellationReasons { get; }

        public ICancellationRepository Cancellations { get; }

        public IDriverRepository Driver { get; }

        public IFeedbackRepository Feedbacks { get; }

        public IMessageRepository Messages { get; }

        public IPaymentRepository Payments { get; }

        public IPriceTableRepository PriceTables { get; }

        public ITransactionRepository Transactions { get; }

        public ITripMatchRepository TripMatches { get; }

        public ITripRequestRepository TripRequests { get; }

        public IUserRepository Users { get; }

        public IVehicleRepository Vehicles { get; }

        public IWalletRepository Wallets { get; }

        public IZoneRepository Zones { get; }

        public FDriverContext GetDbContext()
        {
            return _context;
        }

        public UnitOfWork(FDriverContext context, ICancellationReasonRepository cancellationReasons, ICancellationRepository cancellations, IDriverRepository driver, IFeedbackRepository feedbacks, IMessageRepository messages, IPaymentRepository payments, IPriceTableRepository priceTables, ITransactionRepository transactions, ITripMatchRepository tripMatches, ITripRequestRepository tripRequests, IUserRepository users, IVehicleRepository vehicles, IWalletRepository wallets, IZoneRepository zones)
        {
            _context = context;
            CancellationReasons = cancellationReasons;
            Cancellations = cancellations;
            Driver = driver;
            Feedbacks = feedbacks;
            Messages = messages;
            Payments = payments;
            PriceTables = priceTables;
            Transactions = transactions;
            TripMatches = tripMatches;
            TripRequests = tripRequests;
            Users = users;
            Vehicles = vehicles;
            Wallets = wallets;
            Zones = zones;
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();

        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                var result = await _context.SaveChangesAsync();

                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }

                return result;
            }
            catch
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }

                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

       
    }

}

