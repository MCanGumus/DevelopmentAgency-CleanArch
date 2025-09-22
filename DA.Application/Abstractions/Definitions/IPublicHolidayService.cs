using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IPublicHolidayService : IBaseService<PublicHoliday, UpdatePublicHolidayDto, SavePublicHolidayDto, PublicHolidayDto>
    {
        
    }
}
