using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IAcademyInfoService : IBaseService<AcademyInfo, UpdateAcademyInfoDto, SaveAcademyInfoDto, AcademyInfoDto>
    {
        List<AcademyInfoDto> GetAllUserAcademyInfos(Guid id);
    }
}
