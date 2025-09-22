using DA.Domain.Dtos;

namespace DA.Models
{
    public class VehicleRequestsWithEmployees
    {
        public List<EmployeeDto> Employees { get; set; }
        public List<VehicleRequestDto> Requests { get; set; }
    }
}
