using AutoMapper;
using DA.Domain.Dtos;
using DA.Domain.Entities;
//using DA.Domain.Dtos.DefinitionsDto.Definitions;
//using DA.Domain.Entities.Definitions;

namespace DA.Application.Mapper
{
    public class DefinitionsProfile : Profile
    {
        public DefinitionsProfile()
        {

            #region Exam
            CreateMap<Exam, ExamDto>().ReverseMap();
            CreateMap<Exam, UpdateExamDto>().ReverseMap();
            CreateMap<Exam, SaveExamDto>().ReverseMap();
            #endregion

            #region Department
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDto>().ReverseMap();
            CreateMap<Department, SaveDepartmentDto>().ReverseMap();
            #endregion

            #region Apellation
            CreateMap<Apellation, ApellationDto>().ReverseMap();
            CreateMap<Apellation, UpdateApellationDto>().ReverseMap();
            CreateMap<Apellation, SaveApellationDto>().ReverseMap();
            #endregion

            #region Public Holiday
            CreateMap<PublicHoliday, PublicHolidayDto>().ReverseMap();
            CreateMap<PublicHoliday, UpdatePublicHolidayDto>().ReverseMap();
            CreateMap<PublicHoliday, SavePublicHolidayDto>().ReverseMap();
            #endregion

            #region Mission Type
            CreateMap<MissionType, MissionTypeDto>().ReverseMap();
            CreateMap<MissionType, UpdateMissionTypeDto>().ReverseMap();
            CreateMap<MissionType, SaveMissionTypeDto>().ReverseMap();
            #endregion

            #region Educational Institution
            CreateMap<EducationalInstitution, EducationalInstitutionDto>().ReverseMap();
            CreateMap<EducationalInstitution, UpdateEducationalInstitutionDto>().ReverseMap();
            CreateMap<EducationalInstitution, SaveEducationalInstitutionDto>().ReverseMap();
            #endregion

            #region Department Staff
            CreateMap<DepartmentStaff, DepartmentStaffDto>().ReverseMap();
            CreateMap<DepartmentStaff, UpdateDepartmentStaffDto>().ReverseMap();
            CreateMap<DepartmentStaff, SaveDepartmentStaffDto>().ReverseMap();
            #endregion

        }
    }
}
