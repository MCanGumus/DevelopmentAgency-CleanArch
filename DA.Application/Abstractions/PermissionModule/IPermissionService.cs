using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IPermissionService : IBaseService<Permission, UpdatePermissionDto, SavePermissionDto, PermissionDto>
    {
        List<PermissionDto> GetAllPermissionsByEmployee(Guid id);
        List<PermissionDto> GetAllPermissionsByDepartment(Guid id, DateTime startDate, DateTime endDate);
        Permission GetPermission(Guid id);
        PermissionDto GetPermissionByDocumentId(string id);
        List<PermissionDto> GetPermissionsMonthly(DateTime filter);
        List<PermissionDto> GetPermissionsByProxy(Guid id);
        List<PermissionDto> GetAllPermissionsByFilter(DateTime startDate, DateTime endDate);
    }
}
