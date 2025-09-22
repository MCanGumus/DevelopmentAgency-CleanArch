using DA.Domain.Dtos;
using DA.Domain.Entities.OldDatas;

namespace DA.Models
{
    public class OldMissionsModel
    {
        public List<OldMissions> Missions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
