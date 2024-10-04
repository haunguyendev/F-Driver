using AutoMapper;
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
            var query = _unitOfWork.TripMatchs.FindAll(false,
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
    }
}
