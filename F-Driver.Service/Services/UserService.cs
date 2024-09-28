using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Helpers;
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
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DriverService _driverService;
        private readonly FirebaseService _firebaseService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, DriverService driverService, FirebaseService firebaseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _driverService = driverService;
            _firebaseService = firebaseService;
        }

        #region create user is a driver
        public async Task<bool> CreateUserAsync(UserModel userRequest)
        {
            try
            {
                // Map từ UserModel sang User entity
                var user = _mapper.Map<User>(userRequest);
                if (userRequest.Driver != null)
                {
                    var driver = _mapper.Map<Driver>(userRequest.Driver);
                    user.Driver = driver;

                    if (userRequest.Driver.Vehicles != null && userRequest.Driver.Vehicles.Any())
                    {
                        var vehicles = _mapper.Map<List<Vehicle>>(userRequest.Driver.Vehicles);
                        user.Driver.Vehicles = vehicles;
                    }
                }


                await _unitOfWork.Users.CreateAsync(user);

                await _unitOfWork.CommitAsync();
                userRequest.Id = user.Id;
                if (user.Driver != null)
                {
                    userRequest.Driver.Id = user.Driver.Id; 
                    userRequest.Driver.UserId = user.Id; 
                    if (user.Driver.Vehicles != null && user.Driver.Vehicles.Any())
                    {
                        var vehiclesList = user.Driver.Vehicles.ToList(); 
                        for (int i = 0; i < userRequest.Driver.Vehicles.Count; i++)
                        {
                            userRequest.Driver.Vehicles.ElementAt(i).Id = vehiclesList[i].Id;
                        }
                    }
                }

                if (userRequest.ProfileImageUrl != null && userRequest.ProfileImageUrl.Length > 0)
                {
                    var profileImagePath = $"USER/{user.Id}/ProfileImage";
                    user.ProfileImageUrl = await _firebaseService.UploadFileToFirebase(userRequest.ProfileImageUrl, profileImagePath);
                }

                if (userRequest.StudentIdCardUrl != null && userRequest.StudentIdCardUrl.Length > 0)
                {
                    var studentIdCardPath = $"USER/{user.Id}/StudentIdCard";
                    user.StudentIdCardUrl = await _firebaseService.UploadFileToFirebase(userRequest.StudentIdCardUrl, studentIdCardPath);
                }

                if (userRequest.Driver != null)
                {

                    if (userRequest.Driver.LicenseImageUrl != null && userRequest.Driver.LicenseImageUrl.Length > 0)
                    {
                        var licenseImagePath = $"DRIVER/{user.Id}/LicenseImage";
                        user.Driver.LicenseImageUrl = await _firebaseService.UploadFileToFirebase(userRequest.Driver.LicenseImageUrl, licenseImagePath);
                    }

                    if (userRequest.Driver.Vehicles != null && userRequest.Driver.Vehicles.Any())
                    {

                        foreach (var vehicleRequest in userRequest.Driver.Vehicles)
                        {
                            var vehicle = user.Driver.Vehicles.FirstOrDefault(v => v.Id == vehicleRequest.Id);

                            if (vehicleRequest.VehicleImageUrl != null && vehicleRequest.VehicleImageUrl.Length > 0)
                            {
                                var vehicleImagePath = $"VEHICLE/{user.Driver.Id}/{vehicleRequest.LicensePlate}/VehicleImage";
                                vehicle.VehicleImageUrl = await _firebaseService.UploadFileToFirebase(vehicleRequest.VehicleImageUrl, vehicleImagePath);
                            }

                            if (vehicleRequest.RegistrationImageUrl != null && vehicleRequest.RegistrationImageUrl.Length > 0)
                            {
                                var registrationImagePath = $"VEHICLE/{user.Driver.Id}/{vehicleRequest.LicensePlate}/RegistrationImage";
                                vehicle.RegistrationImageUrl = await _firebaseService.UploadFileToFirebase(vehicleRequest.RegistrationImageUrl, registrationImagePath);
                            }
                        }
                    }
                }

                await _unitOfWork.Users.UpdateAsync(user);

                var rs = await _unitOfWork.CommitAsync();
                if (rs > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }
        #endregion

        #region get user by id
        public async Task<UserModel?> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user != null)
            {
                return _mapper.Map<UserModel>(user);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
