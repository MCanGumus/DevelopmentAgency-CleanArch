using DA.Domain.Dtos;

namespace DA.Models
{
    public class VehicleRequestReportModel
    {
        public List<VehicleRequestDto> VehicleRequests { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
