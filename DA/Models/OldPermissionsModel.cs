using DA.Domain.Dtos;
using DA.Domain.Entities.OldDatas;

namespace DA.Models
{
    public class OldPermissionsModel
    {
        public List<OldPermissions> Permissions { get; set; }
        public List<OldEmployees> Employees{ get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
