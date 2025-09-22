using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IMissionTypeService : IBaseService<MissionType, UpdateMissionTypeDto, SaveMissionTypeDto, MissionTypeDto>
    {
        
    }
}
