using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class AcademyInfoService: BaseService<AcademyInfo, UpdateAcademyInfoDto, SaveAcademyInfoDto, AcademyInfoDto>, IAcademyInfoService
    {
        private readonly IReadRepository<AcademyInfo> _readRepository;
        private readonly IWriteRepository<AcademyInfo> _writeRepository;

        private readonly IMapper _mapper;
        public AcademyInfoService(IReadRepository<AcademyInfo> readRepository, IWriteRepository<AcademyInfo> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public List<AcademyInfoDto> GetAllUserAcademyInfos(Guid id)
        {
            var list = _readRepository.GetWhere(x => x.IdEmployeeFK == id && x.DataType == Domain.Enums.EnumDataType.New).ToList();

            List<AcademyInfoDto> dtos = new List<AcademyInfoDto>();

            foreach (var item in list)
                dtos.Add(_mapper.Map<AcademyInfoDto>(item));

            return dtos;
        }
    }
}
