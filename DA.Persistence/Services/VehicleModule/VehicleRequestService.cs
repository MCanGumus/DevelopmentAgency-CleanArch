using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace DA.Persistence.Services
{
    public class VehicleRequestService : BaseService<VehicleRequest, UpdateVehicleRequestDto, SaveVehicleRequestDto, VehicleRequestDto>, IVehicleRequestService
    {
        private readonly IReadRepository<VehicleRequest> _readRepository;
        private readonly IWriteRepository<VehicleRequest> _writeRepository;

        private readonly IMapper _mapper;
        public VehicleRequestService(IReadRepository<VehicleRequest> readRepository, IWriteRepository<VehicleRequest> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _mapper = mapper;
        }

        public List<VehicleRequestDto> GetFullVehicleRequests(DateTime startDate, DateTime endDate)
        {
            var listFull = _readRepository.GetWhere(x => (x.DateOfStart >= startDate && x.DateOfStart <= endDate) || (x.DateOfEnd >= startDate && x.DateOfEnd <= endDate) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Vehicle).Include(x => x.Mission).ThenInclude(x => x.Employee).ToList();

            List<VehicleRequestDto> dtoList = _mapper.Map<List<VehicleRequest>, List<VehicleRequestDto>>(listFull);

            return dtoList;
        }

        public List<VehicleRequestDto> GetFullVehicles(DateTime startDate, DateTime endDate)
        {
            var listFull = _readRepository.GetWhere(x => 
            (x.DateOfStart >= startDate && x.DateOfStart <= endDate) ||
            (x.DateOfEnd >= startDate && x.DateOfEnd <= endDate) ||
            (x.DateOfStart <= startDate && x.DateOfEnd >= endDate) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Vehicle).Include(x => x.Mission).ToList();

            List<VehicleRequestDto> dtoList = _mapper.Map<List<VehicleRequest>, List<VehicleRequestDto>>(listFull);

            return dtoList;
        }

        public List<VehicleRequestDto> GetFullVehiclesMonthly(DateTime filter)
        {
            var listFull = _readRepository.GetWhere(x => ((x.DateOfStart.Month == filter.Month && x.DateOfStart.Year == filter.Year) || (x.DateOfEnd.Month == filter.Month && x.DateOfEnd.Year == filter.Year)) && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Vehicle).Include(x => x.Mission).ThenInclude(x => x.Employee).ToList();

            List<VehicleRequestDto> dtoList = _mapper.Map<List<VehicleRequest>, List<VehicleRequestDto>>(listFull);

            return dtoList;
        }

        public List<VehicleRequestDto> GetMyVehicleRequests(Guid idEmployee)
        {
            var listFull = _readRepository.GetWhere(x => x.IdEmployeeFK == idEmployee && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Vehicle).Include(x => x.Mission).ThenInclude(x => x.Employee).ToList();

            List<VehicleRequestDto> dtoList = _mapper.Map<List<VehicleRequest>, List<VehicleRequestDto>>(listFull);

            return dtoList;
        }

        public VehicleRequestDto GetVehicleRequest(Guid gid)
        {
            var request = _readRepository.GetWhere(x => x.Id == gid && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Vehicle).Include(x => x.Mission).ThenInclude(x => x.Employee).FirstOrDefault();

            VehicleRequestDto vhcRequestDto = _mapper.Map<VehicleRequestDto>(request);

            return vhcRequestDto;
        }

        public List<VehicleRequestDto> GetVehicleRequestByMission(Guid missionId)
        {
            var request = _readRepository.GetWhere(x => x.RequestType == Domain.Enums.EnumRequestType.Mission && x.IdMissionFK == missionId && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Mission).Include(x => x.Vehicle).ToList();

            List<VehicleRequestDto> dto = _mapper.Map<List<VehicleRequest>, List<VehicleRequestDto>>(request);

            return dto;
        }

        public List<VehicleRequest> GetVehicleRequestByMissionEntities(Guid missionId)
        {
            var request = _readRepository.GetWhere(x => x.RequestType == Domain.Enums.EnumRequestType.Mission && x.IdMissionFK == missionId && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Mission).Include(x => x.Vehicle).ToList();

            return request;
        }

        public List<VehicleRequestDto> GetVehicleRequests()
        {
            var listFull = _readRepository.GetWhere(x => x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Vehicle).Include(x => x.Mission).ThenInclude(x => x.Employee).ToList();

            List<VehicleRequestDto> dtoList = _mapper.Map<List<VehicleRequest>, List<VehicleRequestDto>>(listFull);

            return dtoList;
        }

        public List<VehicleRequestDto> GetVehicleRequestsByVehicle(Guid idVehicle)
        {
            var listFull = _readRepository.GetWhere(x => x.IdVehicleFK == idVehicle && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Vehicle).Include(x => x.Mission).ThenInclude(x => x.Employee).ToList();

            List<VehicleRequestDto> dtoList = _mapper.Map<List<VehicleRequest>, List<VehicleRequestDto>>(listFull);

            return dtoList;
        }
    }
}
