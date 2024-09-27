using AutoMapper;
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
            CreateMap<UserModel, User>()
                .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver)) // Ensure Driver is mapped correctly
                .ReverseMap()
                .MaxDepth(1); // Limit recursion depth if needed

            // Mapping for DriverModel <-> Driver
            CreateMap<DriverModel, Driver>()
                .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.Vehicles)) // Ensure Vehicles are mapped
                .ReverseMap()
                .MaxDepth(1); // Limit recursion depth if needed

            // Mapping for VehicleModel <-> Vehicle
            CreateMap<VehicleModel, Vehicle>().ReverseMap();
        }
    }
}
