using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IApellationService : IBaseService<Apellation, UpdateApellationDto, SaveApellationDto, ApellationDto>
    {
        
    }
}
