using AutoMapper;
using DA.Domain.Dtos;
using DA.Domain.Entities;

namespace DA.Application.Mapper
{
    public class MissionModuleProfile : Profile
    {
        public MissionModuleProfile()
        {
            #region Mission
            CreateMap<Mission, MissionDto>().ReverseMap();
            CreateMap<Mission, UpdateMissionDto>().ReverseMap();
            CreateMap<Mission, SaveMissionDto>().ReverseMap();
            #endregion


        }
    }
}
