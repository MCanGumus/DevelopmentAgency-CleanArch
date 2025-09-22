using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class PublicHolidayService: BaseService<PublicHoliday, UpdatePublicHolidayDto, SavePublicHolidayDto, PublicHolidayDto>, IPublicHolidayService
    {
        private readonly IReadRepository<PublicHoliday> _readRepository;
        private readonly IWriteRepository<PublicHoliday> _writeRepository;

        private readonly IMapper _mapper;
        public PublicHolidayService(IReadRepository<PublicHoliday> readRepository, IWriteRepository<PublicHoliday> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }
    }
}
