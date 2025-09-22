using DA.Domain.Dtos;

namespace DA.Models
{
    public class VehicleCalendarModel
    {
        public DateTime Filter { get; set; }
        public Dictionary<DateTime, List<VehicleRequestDto>> Requests { get; set; } = new Dictionary<DateTime, List<VehicleRequestDto>>();
        public Dictionary<Guid, List<VehiclePassengerDto>> Passengers { get; set; } = new Dictionary<Guid, List<VehiclePassengerDto>>();
    }
}
