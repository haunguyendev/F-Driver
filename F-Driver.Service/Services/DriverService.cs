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
    public class DriverService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly VehicleService _vehicleService;

        public DriverService(IUnitOfWork unitOfWork, IMapper mapper, VehicleService vehicleService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vehicleService = vehicleService;
        }

        public async Task<DriverModel> CreateDriverAsync(DriverModel driverRequest, int userId)
        {
            var driver = _mapper.Map<Driver>(driverRequest);
            driver.UserId = userId;

            // Lưu Driver vào database
            var createdDriver = await _unitOfWork.Driver.CreateAsync(driver);
            var result = await _unitOfWork.CommitAsync();

            List<VehicleModel> vehicles = null;

            if (driverRequest.Vehicles != null && driverRequest.Vehicles.Any())
            {
                vehicles = await _vehicleService.CreateVehiclesAsync(driverRequest.Vehicles, driver.Id);
            }

            var driverModel = _mapper.Map<DriverModel>(createdDriver);

            if (vehicles != null)
            {
                driverModel.Vehicles = _mapper.Map<List<VehicleModel>>(vehicles);
            }

            return driverModel;
        }
    }
}
