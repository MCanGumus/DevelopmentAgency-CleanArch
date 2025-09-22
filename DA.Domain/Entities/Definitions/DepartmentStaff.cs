using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class DepartmentStaff : BaseEntity
    {

        public Guid IdEmployeeFK { get; set; }
        public Employee Employee { get; set; }
        public Guid IdApellationFK { get; set; }
        public Apellation Apellation { get; set; }


    }
}
