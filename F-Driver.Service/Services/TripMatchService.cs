﻿using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
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
    public class TripMatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TripMatchService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //get trip match with filter
        public async Task<PaginatedList<TripMatchModel>> GetAllTripMatchesAsync(TripMatchQueryParameters filterRequest)
        {
            var query = _unitOfWork.TripMatches.FindAll(false,
                    t => t.Cancellations,
                    t => t.Feedbacks,
                    t => t.Messages,
                    t => t.Driver,
                    t => t.TripRequest,
                    t => t.Payments
            );

            // Apply filtering
            if (filterRequest.TripRequestId.HasValue)
                query = query.Where(t => t.TripRequestId == filterRequest.TripRequestId.Value);

            if (filterRequest.DriverId.HasValue)
                query = query.Where(t => t.DriverId == filterRequest.DriverId.Value);

            if (filterRequest.MatchedAt.HasValue)
                query = query.Where(t => t.MatchedAt == filterRequest.MatchedAt.Value);

            if (!string.IsNullOrEmpty(filterRequest.Status))
                query = query.Where(t => t.Status == filterRequest.Status);

            // Apply sorting
            if (!string.IsNullOrEmpty(filterRequest.SortBy))
            {
                switch (filterRequest.SortBy.ToLower())
                {
                    case "triprequestid":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.TripRequestId) : query.OrderByDescending(t => t.TripRequestId);
                        break;
                    case "driverid":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.DriverId) : query.OrderByDescending(t => t.DriverId);
                        break;
                    case "matchedat":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.MatchedAt) : query.OrderByDescending(t => t.MatchedAt);
                        break;
                    case "status":
                    default:
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status);
                        break;
                }
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filterRequest.Page - 1) * filterRequest.PageSize)
                .Take(filterRequest.PageSize)
                .ToListAsync();

            var tripMatchModels = _mapper.Map<List<TripMatchModel>>(items);

            return new PaginatedList<TripMatchModel>(tripMatchModels, totalCount, filterRequest.Page, filterRequest.PageSize);
        }

        #region
        public async Task<bool> CreateTripMatchAsync(int tripRequestId,int driverId)
        {
            // Kiểm tra sự tồn tại của TripRequest và Driver
            var tripRequest = await _unitOfWork.TripRequests.FindAsync(t=>t.Id==tripRequestId);
            var driver = await _unitOfWork.Users.FindAsync(d=>d.Id==driverId);

            if (tripRequest == null || driver == null)
            {
                return false;
            }

            // Tạo TripMatch với trạng thái Pending
            var tripMatch = new TripMatch
            {
                TripRequestId =tripRequestId,
                DriverId = driverId,
                Status = TripMatchStatusEnum.Pending,
                MatchedAt = DateTime.UtcNow
            };

            await _unitOfWork.TripMatches.CreateAsync(tripMatch);
            await _unitOfWork.CommitAsync();

            return true;
        }

        #endregion
        #region update trip match status
        public async Task UpdateTripMatchStatusAsync(int tripMatchId, int passengerId, string status)
        {
            // Kiểm tra sự tồn tại của TripMatch
            var tripMatch = await _unitOfWork.TripMatches.FindAsync(tm => tm.Id == tripMatchId);
            if (tripMatch == null)
            {
                throw new EntryPointNotFoundException("Trip match not found.");
            }

            // Kiểm tra sự tồn tại của TripRequest
            var tripRequest = await _unitOfWork.TripRequests.FindAsync(tr => tr.Id == tripMatch.TripRequestId);
            if (tripRequest == null || tripRequest.UserId != passengerId)
            {
                throw new Exception("Unauthorized or trip request not found.");
            }

            // Cập nhật trạng thái trip match
            if (status == TripMatchStatusEnum.Accepted)
            {
                tripMatch.Status = TripMatchStatusEnum.Accepted;
                tripRequest.Status = TripRequestStatusEnum.Completed; // Đánh dấu request là đã hoàn thành
            }
            else if (status == TripMatchStatusEnum.Rejected)
            {
                tripMatch.Status = TripMatchStatusEnum.Rejected;
            }
            else
            {
                throw new Exception("Invalid status.");
            }

            // Lưu thay đổi
            await _unitOfWork.TripMatches.UpdateAsync(tripMatch);
            await _unitOfWork.TripRequests.UpdateAsync(tripRequest);
            await _unitOfWork.CommitAsync();
        }

        #endregion
        #region start trip 
        public async Task<bool> StartTripAsync(int tripMatchId, int driverId)
        {
            var tripMatch = await _unitOfWork.TripMatches.FindAsync(tm => tm.Id == tripMatchId);
            if (tripMatch == null)
            {
                throw new ArgumentException("Trip match not found.");
            }
            if (tripMatch.DriverId != driverId)
            {
                throw new UnauthorizedAccessException("You do not have permission to start this trip.");

            }
            if(tripMatch.Status!= TripMatchStatusEnum.Accepted)
            {
                throw new InvalidOperationException("Trip match is not in a valid state to be started. ");
            }
            tripMatch.Status = TripMatchStatusEnum.InProgress;
            tripMatch.StartedAt=DateTime.UtcNow;
            await _unitOfWork.TripMatches.UpdateAsync(tripMatch);
            await _unitOfWork.CommitAsync();

            return true;
        }
        #endregion
        #region
        public async Task<bool> CompleteTripAsync(int tripMatchId, int driverId)
        {
            // Tìm TripMatch bằng tripMatchId
            var tripMatch = await _unitOfWork.TripMatches.FindAsync(tm => tm.Id == tripMatchId);

            if (tripMatch == null)
            {
                throw new ArgumentException("Trip match not found.");
            }

            // Kiểm tra quyền tài xế
            if (tripMatch.DriverId != driverId)
            {
                throw new UnauthorizedAccessException("You do not have permission to complete this trip.");
            }

            // Kiểm tra trạng thái hiện tại (chỉ có thể hoàn thành nếu đang InProgress)
            if (tripMatch.Status != TripMatchStatusEnum.InProgress)
            {
                throw new InvalidOperationException("Trip match is not in a valid state to be completed.");
            }

            // Cập nhật trạng thái của TripMatch thành Completed
            tripMatch.Status = TripMatchStatusEnum.Completed;
             var tripRequest = await _unitOfWork.TripRequests.FindAsync(tr => tr.Id == tripMatch.TripRequestId);
           

            // Lấy thông tin Passenger và Driver
            var passengerWallet = await _unitOfWork.Wallets.FindAsync(w => w.UserId == tripRequest.UserId);
            var driverWallet = await _unitOfWork.Wallets.FindAsync(w => w.UserId == driverId);

            if (passengerWallet == null || driverWallet == null)
            {
                throw new InvalidOperationException("Wallet not found.");
            }

            // Tính tiền chuyến đi
            decimal tripCost = tripRequest.Price;

            // Kiểm tra số dư ví Passenger
            if (passengerWallet.Balance < tripCost)
            {
                throw new InvalidOperationException("Insufficient balance in passenger's wallet.");
            }

            // Thực hiện trừ tiền từ ví của Passenger
            passengerWallet.Balance -= tripCost;

            // Thực hiện cộng tiền vào ví của Driver
            driverWallet.Balance += tripCost;

            // Cập nhật ví của cả Passenger và Driver
            await _unitOfWork.Wallets.UpdateAsync(passengerWallet);
            await _unitOfWork.Wallets.UpdateAsync(driverWallet);

            // Lưu thông tin Transaction cho Passenger (trừ tiền)
            var passengerTransaction = new Transaction
            {
                WalletId = passengerWallet.Id,
                Amount = -tripCost, // Trừ tiền
                Type = TransactionTypeEnum.TripPayment,
                TransactionDate = DateTime.UtcNow
            };
            await _unitOfWork.Transactions.CreateAsync(passengerTransaction);

            // Lưu thông tin Transaction cho Driver (cộng tiền)
            var driverTransaction = new Transaction
            {
                WalletId = driverWallet.Id,
                Amount = tripCost, // Cộng tiền
                Type = TransactionTypeEnum.TripEarnings,
                TransactionDate = DateTime.UtcNow
            };
            await _unitOfWork.Transactions.CreateAsync(driverTransaction);

            // Lưu thay đổi vào database
            await _unitOfWork.TripMatches.UpdateAsync(tripMatch);
            await _unitOfWork.CommitAsync();

            return true;
        }



        #endregion
        #region cancel trip match

        public async Task CancelTripMatchAsync(int tripMatchId, int reasonId, int userId)
        {
            // Kiểm tra TripMatch có tồn tại không
            var tripMatch = await _unitOfWork.TripMatches.FindAsync(tm=>tm.Id==tripMatchId);
            
            if (tripMatch == null)
                throw new KeyNotFoundException("TripMatch not found");
            var tripRequest = await _unitOfWork.TripRequests.FindAsync(tr => tr.Id == tripMatch.TripRequestId);

            // Kiểm tra quyền của passenger hoặc driver (so khớp với userId)
            if (tripRequest.UserId != userId && tripMatch.DriverId != userId)
                throw new UnauthorizedAccessException("User is not authorized to cancel this trip match.");

            // Cập nhật trạng thái của TripMatch thành "Canceled"
            tripMatch.Status = "Canceled";
            await _unitOfWork.TripMatches.UpdateAsync(tripMatch);

            // Lưu lý do hủy
            var cancellation = new Cancellation
            {
                TripMatchId = tripMatchId,
                ReasonId = reasonId,
                CanceledAt = DateTime.UtcNow
            };
            await _unitOfWork.Cancellations.CreateAsync(cancellation);

            // Lưu thay đổi vào database
            await _unitOfWork.CommitAsync   ();
        }
        #endregion



    }
}
