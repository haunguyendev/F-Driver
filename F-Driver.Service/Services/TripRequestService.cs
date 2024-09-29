using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
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
    }
}
