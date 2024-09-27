using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Repository.Repositories;
using F_Driver.Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class VehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<VehicleModel>> CreateVehiclesAsync(List<VehicleModel> vehicleRequests, int driverId)
        {
            if (vehicleRequests == null || !vehicleRequests.Any())
            {
                return new List<VehicleModel>();
            }
            foreach (var vehicleRequest in vehicleRequests)
            {
                var vehicleModel = _mapper.Map<Vehicle>(vehicleRequest);
                vehicleModel.DriverId = driverId;

                await _unitOfWork.Vehicles.CreateAsync(vehicleModel);
            }
            await _unitOfWork.CommitAsync();
            return vehicleRequests;
        }
    }
}
