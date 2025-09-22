using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IAddressService : IBaseService<Address, UpdateAddressDto, SaveAddressDto, AddressDto>
    {
        List<AddressDto> GetAllMyAddresses(Guid id);
    }
}
