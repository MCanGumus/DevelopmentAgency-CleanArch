using DA.Application.Abstractions;
using DA.Application.Abstractions.Authority;
using DA.Application.Repositories;
using DA.Application.Repositories.Authority.FamilyMemberRepository;
using DA.Application.Repositories.OldEntities;
using DA.Persistence.Repositories;
using DA.Persistence.Repositories.Authority.FamilyMemberRepository;
using DA.Persistence.Repositories.EducationalInstitutionRepository;
using DA.Persistence.Repositories.OldEntities;
using DA.Persistence.Services;
using DA.Persistence.Services.Authority;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace DA.Persistence
{
    public static class CustomExtensionPersistence
    {
        public static void AddContainerWithDependenciesPersistence(this IServiceCollection services)
        {

            #region Employee
            services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();
            services.AddScoped<IEmployeeWriteRepository, EmployeeWriteRepository>();
            #endregion


            #region Exam
            services.AddScoped<IExamReadRepository, ExamReadRepository>();
            services.AddScoped<IExamWriteRepository, ExamWriteRepository>();
            #endregion


            #region Department
            services.AddScoped<IDepartmentReadRepository, DepartmentReadRepository>();
            services.AddScoped<IDepartmentWriteRepository, DepartmentWriteRepository>();
            #endregion


            #region Apellation
            services.AddScoped<IApellationReadRepository, ApellationReadRepository>();
            services.AddScoped<IApellationWriteRepository, ApellationWriteRepository>();
            #endregion


            #region Public Holiday
            services.AddScoped<IPublicHolidayReadRepository, PublicHolidayReadRepository>();
            services.AddScoped<IPublicHolidayWriteRepository, PublicHolidayWriteRepository>();
            #endregion


            #region Mission Type
            services.AddScoped<IMissionTypeReadRepository, MissionTypeReadRepository>();
            services.AddScoped<IMissionTypeWriteRepository, MissionTypeWriteRepository>();
            #endregion


            #region Educational Institution
            services.AddScoped<IEducationalInstitutionReadRepository, EducationalInstitutionReadRepository>();
            services.AddScoped<IEducationalInstitutionWriteRepository, EducationalInstitutionWriteRepository>();
            #endregion


            #region Address
            services.AddScoped<IAddressReadRepository, AddressReadRepository>();
            services.AddScoped<IAddressWriteRepository, AddressWriteRepository>();
            #endregion


            #region GSMNumber
            services.AddScoped<IGSMNumberReadRepository, GSMNumberReadRepository>();
            services.AddScoped<IGSMNumberWriteRepository, GSMNumberWriteRepository>();
            #endregion


            #region EMail
            services.AddScoped<IEMailReadRepository, EMailReadRepository>();
            services.AddScoped<IEMailWriteRepository, EMailWriteRepository>();
            #endregion


            #region Department Staff
            services.AddScoped<IDepartmentStaffReadRepository, DepartmentStaffReadRepository>();
            services.AddScoped<IDepartmentStaffWriteRepository, DepartmentStaffWriteRepository>();
            #endregion


            #region Permission
            services.AddScoped<IPermissionReadRepository, PermissionReadRepository>();
            services.AddScoped<IPermissionWriteRepository, PermissionWriteRepository>();
            #endregion


            #region Vehicle
            services.AddScoped<IVehicleReadRepository, VehicleReadRepository>();
            services.AddScoped<IVehicleWriteRepository, VehicleWriteRepository>();
            #endregion


            #region AcademyInfo
            services.AddScoped<IAcademyInfoReadRepository, AcademyInfoReadRepository>();
            services.AddScoped<IAcademyInfoWriteRepository, AcademyInfoWriteRepository>();
            #endregion


            #region Mission
            services.AddScoped<IMissionReadRepository, MissionReadRepository>();
            services.AddScoped<IMissionWriteRepository, MissionWriteRepository>();
            #endregion


            #region Vehicle Request
            services.AddScoped<IVehicleRequestReadRepository, VehicleRequestReadRepository>();
            services.AddScoped<IVehicleRequestWriteRepository, VehicleRequestWriteRepository>();
            #endregion


            #region Vehicle Passenger
            services.AddScoped<IVehiclePassengerReadRepository, VehiclePassengerReadRepository>();
            services.AddScoped<IVehiclePassengerWriteRepository, VehiclePassengerWriteRepository>();
            #endregion

            #region LogEntry
            services.AddScoped<ILogEntryReadRepository, LogEntryReadRepository>();
            services.AddScoped<ILogEntryWriteRepository, LogEntryWriteRepository>();
            #endregion

            #region Old Datas
            services.AddScoped<IOldMissionsReadRepository, OldMissionsReadRepository>();
            services.AddScoped<IOldEmployeesReadRepository, OldEmployeesReadRepository>();
            services.AddScoped<IOldPermissionsReadRepository, OldPermissionsReadRepository>();
            #endregion

            #region Family Members
            services.AddScoped<IFamilyMemberReadRepository, FamilyMemberReadRepository>();
            services.AddScoped<IFamilyMemberWriteRepository, FamilyMemberWriteRepository>();
            #endregion

            #region  services

            #region Base
            services.AddScoped(typeof(IBaseService<,,,>), typeof(BaseService<,,,>));
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            #endregion


            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IApellationService, ApellationService>();
            services.AddScoped<IPublicHolidayService, PublicHolidayService>();
            services.AddScoped<IMissionTypeService, MissionTypeService>();
            services.AddScoped<IEducationalInstitutionService, EducationalInstitutionService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IGSMNumberService, GSMNumberService>();
            services.AddScoped<IEMailService, EMailService>();
            services.AddScoped<IDepartmentStaffService, DepartmentStaffService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IAcademyInfoService, AcademyInfoService>();
            services.AddScoped<IMissionService, MissionService>();
            services.AddScoped<IVehicleRequestService, VehicleRequestService>();
            services.AddScoped<IVehiclePassengerService, VehiclePassengerService>();
            services.AddScoped<ILogEntryService, LogEntryService>();
            services.AddScoped<IFamilyMemberService, FamilyMemberService>();
            #endregion
        }
    }
}
