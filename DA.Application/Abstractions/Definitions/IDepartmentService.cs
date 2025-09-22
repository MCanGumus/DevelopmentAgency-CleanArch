using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IDepartmentService : IBaseService<Department, UpdateDepartmentDto, SaveDepartmentDto, DepartmentDto>
    {
        Department GetDepartmentWithEmployee(Guid id);

        Department GetPresidentOfDepartment(Guid idEmployee);
        Department GetHelperOfDepartment(Guid idEmployee);

        List<Department> GetAllDepartmentWithEmployee();
    }
}
