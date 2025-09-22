using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class GSMNumberService: BaseService<GSMNumber, UpdateGSMNumberDto, SaveGSMNumberDto, GSMNumberDto>, IGSMNumberService
    {
        private readonly IReadRepository<GSMNumber> _readRepository;
        private readonly IWriteRepository<GSMNumber> _writeRepository;

        private readonly IMapper _mapper;
        public GSMNumberService(IReadRepository<GSMNumber> readRepository, IWriteRepository<GSMNumber> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public List<GSMNumberDto> GetAllMyNumbers(Guid id)
        {
            var list = _readRepository.GetWhere(x => x.IdEmployeeFK == id && x.DataType == Domain.Enums.EnumDataType.New).ToList();

            List<GSMNumberDto> lstGsm = new List<GSMNumberDto>();

            foreach (var item in list)
                lstGsm.Add(_mapper.Map<GSMNumberDto>(item));

            return lstGsm;
        }
    }
}
