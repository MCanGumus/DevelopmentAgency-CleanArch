using AutoMapper;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Application.Mapper
{
    public class VehicleModuleProfile : Profile
    {
        public VehicleModuleProfile()
        {
            #region Vehicle
            CreateMap<Vehicle, VehicleDto>().ReverseMap();
            CreateMap<Vehicle, UpdateVehicleDto>().ReverseMap();
            CreateMap<Vehicle, SaveVehicleDto>().ReverseMap();
            #endregion

            #region Vehicle Request
            CreateMap<VehicleRequest, VehicleRequestDto>().ReverseMap();
            CreateMap<VehicleRequest, UpdateVehicleRequestDto>().ReverseMap();
            CreateMap<VehicleRequest, SaveVehicleRequestDto>().ReverseMap();
            #endregion

            #region Vehicle Passenger
            CreateMap<VehiclePassenger, VehiclePassengerDto>().ReverseMap();
            CreateMap<VehiclePassenger, UpdateVehiclePassengerDto>().ReverseMap();
            CreateMap<VehiclePassenger, SaveVehiclePassengerDto>().ReverseMap();
            #endregion

        }
    }
}