using DA.Domain.Dtos;

namespace DA.Models
{
    public class PermissionsWithEmployees
    {
        public List<EmployeeDto> Employees { get; set; }
        public List<PermissionDto> Permissions { get; set; }
        public List<PublicHolidayDto> Holidays { get; set; }
        public List<AddressDto> Addresses { get; set; }
    }
}
