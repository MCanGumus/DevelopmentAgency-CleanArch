using DA.Domain.Dtos;

namespace DA.Models
{
    public class DepartmentStaffModel
    {
        public List<DepartmentStaffDto> DepartmentStaffs { get; set; }
        public DepartmentAndEmployees DepartmentAndEmployees { get; set; }
        public List<ApellationDto> Apellations { get; set; }
    }
}
