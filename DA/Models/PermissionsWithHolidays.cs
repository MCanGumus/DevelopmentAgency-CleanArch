using DA.Domain.Dtos;

namespace DA.Models
{
    public class PermissionsWithHolidays
    {
        public List<PublicHolidayDto> Holidays{ get; set; }
        public List<PermissionDto> Permissions { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
