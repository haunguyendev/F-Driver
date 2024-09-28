﻿using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Helpers;
using F_Driver.Repository.Interfaces;
using F_Driver.Repository.Repositories;
using F_Driver.Service.BusinessModels;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace F_Driver.Service.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DriverService _driverService;
        private readonly FirebaseService _firebaseService;
        private readonly EmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, DriverService driverService, FirebaseService firebaseService, EmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _driverService = driverService;
            _firebaseService = firebaseService;
            _emailService = emailService;
        }

        private readonly Dictionary<int, string> _statusCodeDetails = new Dictionary<int, string>
    {
        { 0, "SUCCESS" },
        { 1, "Driver's license number does not match driver's license image" },
        { 2, "Driver's license image is blurry or incorrect" },
        { 3, "The license plate number does not match the vehicle registration image" },
        { 4, "Vehicle type does not match vehicle registration image" },
        { 5, "Vehicle registration number does not match vehicle registration image" },
        { 6, "Vehicle image is blurry or incorrect" },
        { 7, "Vehicle registration image is blurred or incorrect" }
    };

        #region create user is a driver
        public async Task<bool> CreateUserAsync(CreateUserModel userRequest)
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

        #region get update verify status user
        public async Task<List<string>> HandleStatusVerifyCodes(List<int> errorCodes, int userId)
        {
            var user = await _unitOfWork.Users.FindByCondition(u => u.Id == userId).FirstOrDefaultAsync();
            // Check if the list contains the success code (0)
            if (errorCodes.Contains(0) && errorCodes.Count > 1)
            {
                // If 0 is found along with other error codes, return an error
                throw new InvalidOperationException("SUCCESS code cannot be combined with other error codes.");
            }

            if (errorCodes.Contains(0))
            {
                if (user != null)
                {
                    user.Verified = true;
                    user.VerificationStatus = "Verified";

                    if (user.Driver != null)
                    {
                        user.Driver.Verified = true;

                        if (user.Driver.Vehicles != null)
                        {
                            foreach (var vehicle in user.Driver.Vehicles)
                            {
                                vehicle.IsVerified = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (user != null)
                {
                    user.VerificationStatus = "Reject";
                }
            }

            if (user != null)
            {
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CommitAsync();
            }
            var errorDetails = new List<string>();
            foreach (var code in errorCodes)
            {
                if (_statusCodeDetails.TryGetValue(code, out var detail))
                {
                    errorDetails.Add(detail);
                }
                else
                {
                    errorDetails.Add("Unknown error code: " + code);
                }
            }
            await SendMailAsync(user.Email, errorDetails);

            return errorDetails;
        }
        #endregion

        #region send mail
        public async Task<bool> SendMailAsync(string email, List<string> message)
        {
            try
            {
                var mailData = new MailData
                {
                    EmailToId = email,
                    EmailToName = email,
                    EmailSubject = "Confirm verify user",
                    EmailBody = string.Join("\n", message)
                };
                return await _emailService.SendEmailAsync(mailData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                return false;
            }
        }
        #endregion
    }
}
