using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class Apellation : BaseEntity
    {


        public string Name { get; set; } = string.Empty;

        public ICollection<DepartmentStaff> DepartmentStaffs { get; set; }
    }
}
