using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace DA.Persistence.Services
{
    public class MissionService: BaseService<Mission, UpdateMissionDto, SaveMissionDto, MissionDto>, IMissionService
    {
        private readonly IReadRepository<Mission> _readRepository;
        private readonly IWriteRepository<Mission> _writeRepository;

        private readonly IMapper _mapper;
        public MissionService(IReadRepository<Mission> readRepository, IWriteRepository<Mission> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public List<MissionDto> GetAllMissions(DateTime startDate, DateTime endDate)
        {
            var listFull = _readRepository.GetWhere(x => ((x.DateOfStart >= startDate && x.DateOfStart <= endDate) || (x.DateOfEnd >= startDate && x.DateOfEnd <= endDate)) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ToList();

            List<MissionDto> dtoList = _mapper.Map<List<Mission>, List<MissionDto>>(listFull);

            return dtoList;
        }

        public List<MissionDto> GetAllMissions()
        {
            var listFull = _readRepository.GetWhere(x => x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ToList();

            List<MissionDto> dtoList = _mapper.Map<List<Mission>, List<MissionDto>>(listFull);

            return dtoList;
        }

        public List<MissionDto> GetAllMissionsByProxy(Guid id)
        {
            var list = _readRepository.GetWhere(
                x => 
                x.Employee.AuthorizationStatus == Domain.Enums.EnumAuthorizationStatus.Admin && 
                x.IdProxyFK == id &&
                x.State == Domain.Enums.EnumState.Accepted &&
                x.DateOfStart <= DateTime.Now &&
                x.DateOfEnd >= DateTime.Now &&
                x.MissionType != Domain.Enums.EnumMissionType.BolgeIci &&
                x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ThenInclude(x => x.Department).ToList();

            List<MissionDto> entityList = _mapper.Map<List<Mission>, List<MissionDto>>(list);

            return entityList;
        }

        public List<MissionDto> GetAllMissionsDepartment(Guid idDepartment, DateTime startDate, DateTime endDate)
        {
            var listFull = _readRepository.GetWhere(x => x.Employee.IdDepartmentFK != null && x.Employee.IdDepartmentFK == idDepartment && ((x.DateOfStart >= startDate && x.DateOfStart <= endDate) || (x.DateOfEnd >= startDate && x.DateOfEnd <= endDate)) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ThenInclude(x => x.Department).ToList();

            List<MissionDto> dtoList = _mapper.Map<List<Mission>, List<MissionDto>>(listFull);

            return dtoList;
        }

        public List<MissionDto> GetAllMissionsMonthly(DateTime filter)
        {
            var listFull = _readRepository.GetWhere(x => ((x.DateOfStart.Month == filter.Month && x.DateOfStart.Year == filter.Year) || (x.DateOfEnd.Month == filter.Month && x.DateOfEnd.Year == filter.Year)) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ToList();

            List<MissionDto> dtoList = _mapper.Map<List<Mission>, List<MissionDto>>(listFull);

            return dtoList;
        }

        public List<MissionDto> GetAllMissionsOfMe(Guid gid)
        {
            var list = _readRepository.GetWhere(x =>x.IdEmployeeFK == gid && x.DataType == Domain.Enums.EnumDataType.New).ToList();

            List<MissionDto> entityList = _mapper.Map<List<Mission>, List<MissionDto>>(list);

            return entityList;
        }

        public MissionDto GetMission(Guid gid)
        {
            var entity = _readRepository.GetWhere(x => x.Id == gid).Include(x => x.Proxy).Include(x => x.Employee).ThenInclude(x => x.Department).ThenInclude(x => x.Employee).FirstOrDefault();

            MissionDto dto = _mapper.Map<MissionDto>(entity);

            return dto;
        }

        public MissionDto GetMissionWithDocId(string docId)
        {
            var entity = _readRepository.GetWhere(x => x.DocumentId == docId).FirstOrDefault();

            MissionDto dto = _mapper.Map<MissionDto>(entity);

            return dto;
        }
    }
}
