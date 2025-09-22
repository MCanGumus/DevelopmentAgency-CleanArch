using DA.Domain.Dtos;

namespace DA.Models
{
    public class MissionsAndEmployees
    {
        public List<MissionDto> Missions{ get; set; }
        public List<EmployeeDto> Employees { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
