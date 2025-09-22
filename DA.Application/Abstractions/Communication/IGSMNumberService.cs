using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IGSMNumberService : IBaseService<GSMNumber, UpdateGSMNumberDto, SaveGSMNumberDto, GSMNumberDto>
    {
        List<GSMNumberDto> GetAllMyNumbers(Guid id);
    }
}
