using DA.Domain.Dtos;

namespace DA.Models
{
    public class MissionReportModel
    {
        public List<MissionDto> Missions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate{ get; set; }
    }
}
