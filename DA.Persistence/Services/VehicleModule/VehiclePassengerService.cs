using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DA.Persistence.Services
{
    public class VehiclePassengerService: BaseService<VehiclePassenger, UpdateVehiclePassengerDto, SaveVehiclePassengerDto, VehiclePassengerDto>, IVehiclePassengerService
    {
        private readonly IReadRepository<VehiclePassenger> _readRepository;
        private readonly IWriteRepository<VehiclePassenger> _writeRepository;

        private readonly IMapper _mapper;
        public VehiclePassengerService(IReadRepository<VehiclePassenger> readRepository, IWriteRepository<VehiclePassenger> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public List<VehiclePassengerDto> GetVehiclePassengers(Guid requestId)
        {
            var listFull = _readRepository.GetWhere(x => x.IdVehicleRequestFK == requestId && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ToList();

            List<VehiclePassengerDto> dtoList = _mapper.Map<List<VehiclePassenger>, List<VehiclePassengerDto>>(listFull);

            return dtoList;
        }
    }
}
