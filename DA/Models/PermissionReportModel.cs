using DA.Domain.Dtos;

namespace DA.Models
{
    public class PermissionReportModel
    {
        public List<PermissionDto> Permissions{ get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
