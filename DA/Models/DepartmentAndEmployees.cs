using DA.Domain.Dtos;
using DA.Domain.Entities;

namespace DA.Models
{
    public class DepartmentAndEmployees
    {
        public List<EmployeeDto> Employees { get; set; }
        public List<Department> Departments { get; set; }
    }
}
