using DA.Domain.Dtos;

namespace DA.Models
{
    public class VehicleCalendarWeek
    {
        public DateTime Day {  get; set; }
        public string Plate { get; set; }
        public string Employee { get; set; }
        public List<VehiclePassengerDto> Passengers { get; set; } = new List<VehiclePassengerDto>();
    }
}
