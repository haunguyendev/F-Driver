using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Helpers;
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

        #region get user filter
        public async Task<PaginatedList<TripRequest>> GetTripRequestsAsync(TripRequestQueryParameters parameters)
        {
            // Query cơ bản
            var query = _unitOfWork.TripRequests.FindAll();

            // Lọc theo UserId
            if (parameters.UserId.HasValue)
            {
                query = query.Where(tr => tr.UserId == parameters.UserId.Value);
            }

            // Lọc theo FromZoneId
            if (parameters.FromZoneId.HasValue)
            {
                query = query.Where(tr => tr.FromZoneId == parameters.FromZoneId.Value);
            }

            // Lọc theo ToZoneId
            if (parameters.ToZoneId.HasValue)
            {
                query = query.Where(tr => tr.ToZoneId == parameters.ToZoneId.Value);
            }

            // Lọc theo TripDate
            if (parameters.TripDate.HasValue)
            {
                query = query.Where(tr => tr.TripDate == parameters.TripDate.Value);
            }

            // Lọc theo StartTime
            if (parameters.StartTime.HasValue)
            {
                query = query.Where(tr => tr.StartTime == parameters.StartTime.Value);
            }

            // Lọc theo Status
            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                query = query.Where(tr => tr.Status == parameters.Status);
            }

            // Tính toán số lượng phần tử
            var totalCount = await query.CountAsync();

            // Phân trang
            var tripRequests = await query.Skip((parameters.Page - 1) * parameters.PageSize)
                                           .Take(parameters.PageSize)
                                           .ToListAsync();

            return new PaginatedList<TripRequest>(tripRequests, totalCount, parameters.Page, parameters.PageSize);
        }
 
        #endregion
    }
}
