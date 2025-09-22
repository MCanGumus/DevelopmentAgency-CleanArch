using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DA.Persistence.Services
{
    public class PermissionService: BaseService<Permission, UpdatePermissionDto, SavePermissionDto, PermissionDto>, IPermissionService
    {
        private readonly IReadRepository<Permission> _readRepository;
        private readonly IWriteRepository<Permission> _writeRepository;

        private readonly IMapper _mapper;
        public PermissionService(IReadRepository<Permission> readRepository, IWriteRepository<Permission> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public List<PermissionDto> GetAllPermissionsByEmployee(Guid id)
        {
            var myPermissions = _readRepository.GetWhere(x => x.IdEmployeeFK == id && x.DataType == Domain.Enums.EnumDataType.New).ToList();

            List<PermissionDto> permissionsDto = new List<PermissionDto>();

            foreach (var permission in myPermissions)
                permissionsDto.Add(_mapper.Map<PermissionDto>(permission));

            return permissionsDto;
        }

        public List<PermissionDto> GetAllPermissionsByDepartment(Guid id, DateTime startDate, DateTime endDate)
        {
            var myPermissions = _readRepository.GetWhere(x => x.IdDepartmentFK == id && ((x.StartDate >= startDate && x.StartDate <= endDate) || (x.EndDate >= startDate && x.EndDate <= endDate)) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).Include(x => x.Department).ToList();

            List<PermissionDto> permissionsDto = new List<PermissionDto>();

            foreach (var permission in myPermissions)
                permissionsDto.Add(_mapper.Map<PermissionDto>(permission));

            return permissionsDto;
        }

        public Permission GetPermission(Guid id)
        {
            Permission permission = _readRepository.GetWhere(x => x.Id == id && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).Include(x => x.Department).ThenInclude(x => x.Employee).Include(x => x.Delegate).Include(x => x.Proxy).FirstOrDefault();
        
            return permission;
        }

        public PermissionDto GetPermissionByDocumentId(string id)
        {
            Permission permission = _readRepository.GetWhere(x => x.DocumentId == id && x.DataType == Domain.Enums.EnumDataType.New).FirstOrDefault();

            return _mapper.Map<PermissionDto>(permission);
        }

        public List<PermissionDto> GetPermissionsMonthly(DateTime filter)
        {
            var listFull = _readRepository.GetWhere(x => ((x.StartDate.Month == filter.Month && x.StartDate.Year == filter.Year) || (x.EndDate.Month == filter.Month && x.EndDate.Year == filter.Year)) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ToList();

            List<PermissionDto> dtoList = _mapper.Map<List<Permission>, List<PermissionDto>>(listFull);

            return dtoList;
        }

        public List<PermissionDto> GetAllPermissionsByFilter(DateTime startDate, DateTime endDate)
        {
            var listFull = _readRepository.GetWhere(x => ((x.StartDate >= startDate && x.StartDate <= endDate) || (x.EndDate >= startDate && x.EndDate <= endDate)) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ToList();

            List<PermissionDto> dtoList = _mapper.Map<List<Permission>, List<PermissionDto>>(listFull);

            return dtoList;
        }

        public List<PermissionDto> GetPermissionsByProxy(Guid id)
        {
            var myPermissions = _readRepository.GetWhere(x =>
                x.Employee.AuthorizationStatus == Domain.Enums.EnumAuthorizationStatus.Admin &&
                x.IdProxyFK == id &&
                x.State == Domain.Enums.EnumState.Accepted &&
                x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now &&
                x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).Include(x => x.Department).ToList();

            List<PermissionDto> permissionsDto = new List<PermissionDto>();

            foreach (var permission in myPermissions)
                permissionsDto.Add(_mapper.Map<PermissionDto>(permission));

            return permissionsDto;
        }
    }
}
