using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class AddressService: BaseService<Address, UpdateAddressDto, SaveAddressDto, AddressDto>, IAddressService
    {
        private readonly IReadRepository<Address> _readRepository;
        private readonly IWriteRepository<Address> _writeRepository;

        private readonly IMapper _mapper;
        public AddressService(IReadRepository<Address> readRepository, IWriteRepository<Address> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public List<AddressDto> GetAllMyAddresses(Guid id)
        {
            var list = _readRepository.GetWhere(x => x.IdEmployeeFK == id && x.DataType == Domain.Enums.EnumDataType.New).ToList();

            List<AddressDto> lstAddress = new List<AddressDto>();

            foreach (var item in list)
                lstAddress.Add(_mapper.Map<AddressDto>(item));

            return lstAddress;
        }
    }
}
