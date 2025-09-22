using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid IdEmployeeFK { get; set; }
        public Employee Employee { get; set; }

        public Guid? IdBackupManager { get; set; }

        public ICollection<DepartmentStaff> DepartmentStaffs { get; set; }
        public ICollection<Permission> Permissions{ get; set; }
        public ICollection<Employee> Employees { get; set; }

    }
}
