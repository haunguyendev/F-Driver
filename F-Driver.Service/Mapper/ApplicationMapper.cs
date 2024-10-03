﻿using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Mapper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            // Mapping for UserModel <-> User
            CreateMap<CreateUserModel, User>()
                .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver)) // Ensure Driver is mapped correctly
                .ReverseMap()
                .MaxDepth(1); // Limit recursion depth if needed

            // Mapping for DriverModel <-> Driver
            CreateMap<CreateDriverModel, Driver>()
                .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.Vehicles)) // Ensure Vehicles are mapped
                .ReverseMap()
                .MaxDepth(1); // Limit recursion depth if needed

            // Mapping for VehicleModel <-> Vehicle
            CreateMap<CreateVehicleModel, Vehicle>().ReverseMap();

            CreateMap<UserModel,User>().ReverseMap();

            CreateMap<ZoneModel, Zone>().ReverseMap();

            CreateMap<PriceTableModel, PriceTable>().ReverseMap();

            CreateMap<TripRequestModel, TripRequest>().ReverseMap();

            CreateMap<TripMatchModel, TripMatch>().ReverseMap()
                .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver))
                .ForMember(dest => dest.TripRequest, opt => opt.MapFrom(src => src.TripRequest))
                .ForMember(dest => dest.Cancellations, opt => opt.MapFrom(src => src.Cancellations))
                .ForMember(dest => dest.Feedbacks, opt => opt.MapFrom(src => src.Feedbacks))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));
        }
    }
}
