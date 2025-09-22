using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IVehicleRequestService : IBaseService<VehicleRequest, UpdateVehicleRequestDto, SaveVehicleRequestDto, VehicleRequestDto>
    {
        List<VehicleRequestDto> GetVehicleRequests();
        List<VehicleRequestDto> GetVehicleRequestsByVehicle(Guid idVehicle);
        List<VehicleRequestDto> GetMyVehicleRequests(Guid idEmployee);
        List<VehicleRequestDto> GetFullVehicles(DateTime startDate, DateTime endDate);
        List<VehicleRequestDto> GetFullVehiclesMonthly(DateTime filter);
        List<VehicleRequestDto> GetVehicleRequestByMission(Guid missionId);
        List<VehicleRequest> GetVehicleRequestByMissionEntities(Guid missionId);
        VehicleRequestDto GetVehicleRequest(Guid gid);
        List<VehicleRequestDto> GetFullVehicleRequests(DateTime startDate, DateTime endDate);
    }
}
