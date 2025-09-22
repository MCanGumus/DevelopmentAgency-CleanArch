using DA.Application.Validation;
using DA.Application.Validations.Authority.FamilyMember;
using DA.Application.Validations.Definition.EducationalInstitution;
using DA.Domain.Dtos;
using DA.Domain.Dtos.Authority.FamilyMemberDtos;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace DA.Application
{
    public static class CustomExtensionApplication
    {
        public static void AddContainerWithDependenciesApplication(this IServiceCollection services)
        {

            #region Employee
            services.AddTransient<IValidator<EmployeeDto>, EmployeeValidator>();
            services.AddTransient<IValidator<SaveEmployeeDto>, SaveEmployeeValidator>();
            services.AddTransient<IValidator<UpdateEmployeeDto>, UpdateEmployeeValidator>();
            #endregion


            #region Exam
            services.AddTransient<IValidator<ExamDto>, ExamValidator>();
            services.AddTransient<IValidator<SaveExamDto>, SaveExamValidator>();
            services.AddTransient<IValidator<UpdateExamDto>, UpdateExamValidator>();
            #endregion


            #region Department
            services.AddTransient<IValidator<DepartmentDto>, DepartmentValidator>();
            services.AddTransient<IValidator<SaveDepartmentDto>, SaveDepartmentValidator>();
            services.AddTransient<IValidator<UpdateDepartmentDto>, UpdateDepartmentValidator>();
            #endregion


            #region Apellation
            services.AddTransient<IValidator<ApellationDto>, ApellationValidator>();
            services.AddTransient<IValidator<SaveApellationDto>, SaveApellationValidator>();
            services.AddTransient<IValidator<UpdateApellationDto>, UpdateApellationValidator>();
            #endregion


            #region Public Holiday
            services.AddTransient<IValidator<PublicHolidayDto>, PublicHolidayValidator>();
            services.AddTransient<IValidator<SavePublicHolidayDto>, SavePublicHolidayValidator>();
            services.AddTransient<IValidator<UpdatePublicHolidayDto>, UpdatePublicHolidayValidator>();
            #endregion


            #region Mission Type
            services.AddTransient<IValidator<MissionTypeDto>, MissionTypeValidator>();
            services.AddTransient<IValidator<SaveMissionTypeDto>, SaveMissionTypeValidator>();
            services.AddTransient<IValidator<UpdateMissionTypeDto>, UpdateMissionTypeValidator>();
            #endregion


            #region Educational Institution
            services.AddTransient<IValidator<EducationalInstitutionDto>, EducationalInstitutionValidator>();
            services.AddTransient<IValidator<SaveEducationalInstitutionDto>, SaveEducationalInstitutionValidator>();
            services.AddTransient<IValidator<UpdateEducationalInstitutionDto>, UpdateEducationalInstitutionValidator>();
            #endregion


            #region Address
            services.AddTransient<IValidator<AddressDto>, AddressValidator>();
            services.AddTransient<IValidator<SaveAddressDto>, SaveAddressValidator>();
            services.AddTransient<IValidator<UpdateAddressDto>, UpdateAddressValidator>();
            #endregion


            #region GSMNumber
            services.AddTransient<IValidator<GSMNumberDto>, GSMNumberValidator>();
            services.AddTransient<IValidator<SaveGSMNumberDto>, SaveGSMNumberValidator>();
            services.AddTransient<IValidator<UpdateGSMNumberDto>, UpdateGSMNumberValidator>();
            #endregion


            #region EMail
            services.AddTransient<IValidator<EMailDto>, EMailValidator>();
            services.AddTransient<IValidator<SaveEMailDto>, SaveEMailValidator>();
            services.AddTransient<IValidator<UpdateEMailDto>, UpdateEMailValidator>();
            #endregion


            #region Department Staff
            services.AddTransient<IValidator<DepartmentStaffDto>, DepartmentStaffValidator>();
            services.AddTransient<IValidator<SaveDepartmentStaffDto>, SaveDepartmentStaffValidator>();
            services.AddTransient<IValidator<UpdateDepartmentStaffDto>, UpdateDepartmentStaffValidator>();
            #endregion


            #region Permission
            services.AddTransient<IValidator<PermissionDto>, PermissionValidator>();
            services.AddTransient<IValidator<SavePermissionDto>, SavePermissionValidator>();
            services.AddTransient<IValidator<UpdatePermissionDto>, UpdatePermissionValidator>();
            #endregion


            #region Vehicle
            services.AddTransient<IValidator<VehicleDto>, VehicleValidator>();
            services.AddTransient<IValidator<SaveVehicleDto>, SaveVehicleValidator>();
            services.AddTransient<IValidator<UpdateVehicleDto>, UpdateVehicleValidator>();
            #endregion


            #region AcademyInfo
            services.AddTransient<IValidator<AcademyInfoDto>, AcademyInfoValidator>();
            services.AddTransient<IValidator<SaveAcademyInfoDto>, SaveAcademyInfoValidator>();
            services.AddTransient<IValidator<UpdateAcademyInfoDto>, UpdateAcademyInfoValidator>();
            #endregion

            
            #region Mission
            services.AddTransient<IValidator<MissionDto>, MissionValidator>();
            services.AddTransient<IValidator<SaveMissionDto>, SaveMissionValidator>();
            services.AddTransient<IValidator<UpdateMissionDto>, UpdateMissionValidator>();
            #endregion


            #region Vehicle Request
            services.AddTransient<IValidator<VehicleRequestDto>, VehicleRequestValidator>();
            services.AddTransient<IValidator<SaveVehicleRequestDto>, SaveVehicleRequestValidator>();
            services.AddTransient<IValidator<UpdateVehicleRequestDto>, UpdateVehicleRequestValidator>();
            #endregion


            #region Vehicle Passenger
            services.AddTransient<IValidator<VehiclePassengerDto>, VehiclePassengerValidator>();
            services.AddTransient<IValidator<SaveVehiclePassengerDto>, SaveVehiclePassengerValidator>();
            services.AddTransient<IValidator<UpdateVehiclePassengerDto>, UpdateVehiclePassengerValidator>();
            #endregion

            #region FamilyMember
            services.AddTransient<IValidator<FamilyMemberDto>, FamilyMemberValidator>();
            services.AddTransient<IValidator<SaveFamilyMemberDto>, SaveFamilyMemberValidator>();
            services.AddTransient<IValidator<UpdateFamilyMemberDto>, UpdateFamilyMemberValidator>();
            #endregion
        }
    }
}
