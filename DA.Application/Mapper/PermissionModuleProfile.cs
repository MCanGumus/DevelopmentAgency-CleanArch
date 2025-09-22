using AutoMapper;
using DA.Domain.Dtos;
using DA.Domain.Entities;
//using DA.Domain.Dtos.PermissionModuleDto.PermissionModule;
//using DA.Domain.Entities.PermissionModule;

namespace DA.Application.Mapper
{
    public class PermissionModuleProfile : Profile
    {
        public PermissionModuleProfile()
        {
            #region Permission
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<Permission, UpdatePermissionDto>().ReverseMap();
            CreateMap<Permission, SavePermissionDto>().ReverseMap();
            #endregion


        }
    }
}
