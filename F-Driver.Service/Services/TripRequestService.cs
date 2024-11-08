using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BuisnessModels.QueryParameters;
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
    public class TripRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TripRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        private static readonly TimeOnly Slot1Start = new TimeOnly(7, 0);  // 07:00 AM
        private static readonly TimeOnly Slot2Start = new TimeOnly(9, 30); // 09:30 AM
        private static readonly TimeOnly Slot3Start = new TimeOnly(12, 30); // 12:30 PM
        private static readonly TimeOnly Slot4Start = new TimeOnly(15, 0);  // 03:00 PM
        public bool IsStartTimeEarlierThanSlot(TimeOnly startTime, int slot)
        {
            switch (slot)
            {
                case 1:
                    return startTime < Slot1Start;
                case 2:
                    return startTime < Slot2Start;
                case 3:
                    return startTime < Slot3Start;
                case 4:
                    return startTime < Slot4Start;
                default:
                    throw new ArgumentException("Invalid slot number.");
            }
        }

        
        //Create trip request
        public async Task<bool> CreateTripRequest(TripRequestModel tripRequestModel)
        {
            try
            {
                bool isEarlier = IsStartTimeEarlierThanSlot(tripRequestModel.StartTime, tripRequestModel.Slot);
                if (!isEarlier)
                {
                    return false;
                }
                var tripRequest = _mapper.Map<TripRequest>(tripRequestModel);
                await _unitOfWork.TripRequests.CreateAsync(tripRequest);
                var rs = await _unitOfWork.CommitAsync();
                if (rs > 0)
                {
                    return true;
                }
                return false;
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return false;

            }
        }

        //get trips request with filter
        public async Task<PaginatedList<TripRequestModel>> GetTripRequests(TripRequestQueryParameters filterRequest)
        {
            var query = _unitOfWork.TripRequests.FindAll(false, [x => x.FromZone,x=>x.ToZone]);
            if (filterRequest.UserId.HasValue)
                query = query.Where(t => t.UserId == filterRequest.UserId.Value);

            if (filterRequest.FromZoneId.HasValue)
                query = query.Where(t => t.FromZoneId == filterRequest.FromZoneId.Value);

            if (filterRequest.ToZoneId.HasValue)
                query = query.Where(t => t.ToZoneId == filterRequest.ToZoneId.Value);

            if (filterRequest.TripDate.HasValue)
                query = query.Where(t => t.TripDate == filterRequest.TripDate.Value);

            if (filterRequest.StartTime.HasValue)
                query = query.Where(t => t.StartTime == filterRequest.StartTime.Value);

            if (filterRequest.Slot.HasValue)
                query = query.Where(t => t.Slot == filterRequest.Slot.Value);

            if (!string.IsNullOrEmpty(filterRequest.Status))
                query = query.Where(t => t.Status == filterRequest.Status);

            if (filterRequest.CreatedAt.HasValue)
                query = query.Where(t => t.CreatedAt == filterRequest.CreatedAt.Value);

            if (!string.IsNullOrEmpty(filterRequest.SortBy))
            {
                switch (filterRequest.SortBy.ToLower())
                {
                    case "userid":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.UserId) : query.OrderByDescending(t => t.UserId);
                        break;
                    case "fromzoneid":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.FromZoneId) : query.OrderByDescending(t => t.FromZoneId);
                        break;
                    case "tozoneid":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.ToZoneId) : query.OrderByDescending(t => t.ToZoneId);
                        break;
                    case "tripdate":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.TripDate) : query.OrderByDescending(t => t.TripDate);
                        break;
                    case "starttime":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.StartTime) : query.OrderByDescending(t => t.StartTime);
                        break;
                    case "slot":
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.Slot) : query.OrderByDescending(t => t.Slot);
                        break;
                    case "createdat":
                    default:
                        query = filterRequest.IsAscending ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt);
                        break;
                }
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filterRequest.Page - 1) * filterRequest.PageSize)
                .Take(filterRequest.PageSize)
                .ToListAsync();
            var tripRequestModels = _mapper.Map<List<TripRequestModel>>(items);

            return new PaginatedList<TripRequestModel>(tripRequestModels, totalCount, filterRequest.Page, filterRequest.PageSize);
        }

        #region cancel trip request update status
        public async Task<bool> CancelTripRequestAsync(int tripRequestId, int passengerId)
        {
            var tripRequest = await _unitOfWork.TripRequests.FindAsync(t => t.Id == tripRequestId);

            // Kiểm tra xem TripRequest có tồn tại không
            if (tripRequest == null)
            {
                throw new ArgumentException("Trip request not found.");
            }

            // Kiểm tra quyền của passenger
            if (tripRequest.UserId != passengerId)
            {
                throw new UnauthorizedAccessException("You do not have permission to cancel this trip request.");
            }

            // Cập nhật trạng thái thành Canceled
            tripRequest.Status = TripRequestStatusEnum.Canceled;

            await _unitOfWork.TripRequests.UpdateAsync(tripRequest);
            await _unitOfWork.CommitAsync();

            return true;
        }


        #endregion

        //check wallet before create trip request
        public async Task<bool> CheckWallet(TripRequestModel tripRequestModel) {
            var priceThisRequest = await _unitOfWork.PriceTables.FindByCondition(tp => tp.FromZoneId == tripRequestModel.FromZoneId && tp.ToZoneId == tripRequestModel.ToZoneId).FirstOrDefaultAsync();
            if (priceThisRequest == null)
            {
                return false;
            }
            //total price
            var totalPrice = priceThisRequest.UnitPrice;
            var listTripRequest = _unitOfWork.TripRequests.FindAll().Where(t => t.UserId == tripRequestModel.UserId).ToList();
            foreach (var tripRequest in listTripRequest)
            {
                totalPrice += tripRequest.Price;
            }
            var wallet = await _unitOfWork.Wallets.FindByCondition(w => w.UserId == tripRequestModel.UserId).FirstOrDefaultAsync();
            if (wallet == null)
            {
                return false;
            }
            if (wallet.Balance < totalPrice)
            {
                return false;
            }
            return true;
        }

    }
}
