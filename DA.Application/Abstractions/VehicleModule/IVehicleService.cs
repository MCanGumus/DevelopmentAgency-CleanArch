using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IVehicleService : IBaseService<Vehicle, UpdateVehicleDto, SaveVehicleDto, VehicleDto>
    {
        
    }
}
