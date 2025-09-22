using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class ApellationService: BaseService<Apellation, UpdateApellationDto, SaveApellationDto, ApellationDto>, IApellationService
    {
        private readonly IReadRepository<Apellation> _readRepository;
        private readonly IWriteRepository<Apellation> _writeRepository;

        private readonly IMapper _mapper;
        public ApellationService(IReadRepository<Apellation> readRepository, IWriteRepository<Apellation> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }
    }
}
