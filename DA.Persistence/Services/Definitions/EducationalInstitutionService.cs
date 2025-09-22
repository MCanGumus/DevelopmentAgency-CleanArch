using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class EducationalInstitutionService : BaseService<EducationalInstitution, UpdateEducationalInstitutionDto, SaveEducationalInstitutionDto, EducationalInstitutionDto>, IEducationalInstitutionService
    {
        private readonly IReadRepository<EducationalInstitution> _readRepository;
        private readonly IWriteRepository<EducationalInstitution> _writeRepository;

        private readonly IMapper _mapper;
        public EducationalInstitutionService(IReadRepository<EducationalInstitution> readRepository, IWriteRepository<EducationalInstitution> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _mapper = mapper;
        }
    }
}
