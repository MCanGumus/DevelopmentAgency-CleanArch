using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class VehicleService: BaseService<Vehicle, UpdateVehicleDto, SaveVehicleDto, VehicleDto>, IVehicleService
    {
        private readonly IReadRepository<Vehicle> _readRepository;
        private readonly IWriteRepository<Vehicle> _writeRepository;

        private readonly IMapper _mapper;
        public VehicleService(IReadRepository<Vehicle> readRepository, IWriteRepository<Vehicle> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }
    }
}
