using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IDepartmentStaffService : IBaseService<DepartmentStaff, UpdateDepartmentStaffDto, SaveDepartmentStaffDto, DepartmentStaffDto>
    {
        List<DepartmentStaffDto> GetAllDepartmentStaffs();

        DepartmentStaff GetDepartmentStaffById(Guid id);

        DepartmentStaff GetDepartmentStaffByEmployee(Guid idEmployee);
    }
}
