using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class MissionTypeService: BaseService<MissionType, UpdateMissionTypeDto, SaveMissionTypeDto, MissionTypeDto>, IMissionTypeService
    {
        private readonly IReadRepository<MissionType> _readRepository;
        private readonly IWriteRepository<MissionType> _writeRepository;

        private readonly IMapper _mapper;
        public MissionTypeService(IReadRepository<MissionType> readRepository, IWriteRepository<MissionType> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }
    }
}
