using AutoMapper;
using DA.Domain.Dtos;
using DA.Domain.Dtos.Authority.FamilyMemberDtos;
using DA.Domain.Entities;
using DA.Domain.Entities.Authority;
//using DA.Domain.Dtos.AuthorityDto.Authority;
//using DA.Domain.Entities.Authority;

namespace DA.Application.Mapper
{
    public class AuthorityProfile : Profile
    {
        public AuthorityProfile()
        {
            #region Employee
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, UpdateEmployeeDto>().ReverseMap();
            CreateMap<Employee, SaveEmployeeDto>().ReverseMap();
            #endregion

            #region AcademyInfo
            CreateMap<AcademyInfo, AcademyInfoDto>().ReverseMap();
            CreateMap<AcademyInfo, UpdateAcademyInfoDto>().ReverseMap();
            CreateMap<AcademyInfo, SaveAcademyInfoDto>().ReverseMap();
            #endregion

            #region FamilyMember
            CreateMap<FamilyMember, FamilyMemberDto>().ReverseMap();
            CreateMap<FamilyMember, UpdateFamilyMemberDto>().ReverseMap();
            CreateMap<FamilyMember, SaveFamilyMemberDto>().ReverseMap();
            #endregion
        }
    }
}
