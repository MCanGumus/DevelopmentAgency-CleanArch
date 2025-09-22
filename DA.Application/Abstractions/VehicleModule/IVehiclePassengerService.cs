using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IVehiclePassengerService : IBaseService<VehiclePassenger, UpdateVehiclePassengerDto, SaveVehiclePassengerDto, VehiclePassengerDto>
    {
        List<VehiclePassengerDto> GetVehiclePassengers(Guid requestId);
    }
}
