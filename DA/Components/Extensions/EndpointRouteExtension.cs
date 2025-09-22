namespace DA.Components.Extensions
{
    public static class EndpointRouteExtension
    {
        public static void MapCustomRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDefaultControllerRoute();

            endpoints.MapControllerRoute(
            name: "FamilyMembers",
            pattern: "Ailem",
            defaults: new { controller = "FamilyMember", action = "FamilyMember" }
        );

            endpoints.MapControllerRoute(
            name: "DetailedEmployee",
            pattern: "DetayliCalisanBilgileri",
            defaults: new { controller = "DetailedEmployee", action = "DetailedEmployee" }
        );

            endpoints.MapControllerRoute(
            name: "ExitedEmployees",
            pattern: "IstenCikanlar",
            defaults: new { controller = "Employee", action = "EmployeesExited" }
        );

            endpoints.MapControllerRoute(
            name: "OldMissions",
            pattern: "EskiGorevler",
            defaults: new { controller = "OldMissions", action = "OldMissions" }
        );


            endpoints.MapControllerRoute(
            name: "OldEmployees",
            pattern: "EskiCalisanlar",
            defaults: new { controller = "OldEmployees", action = "OldEmployees" }
        );


            endpoints.MapControllerRoute(
            name: "OldPermissions",
            pattern: "EskiIzinler",
            defaults: new { controller = "OldPermissions", action = "OldPermissions" }
        );

            endpoints.MapControllerRoute(
            name: "MissionReport",
            pattern: "GorevRaporlari",
            defaults: new { controller = "MissionReport", action = "MissionReport" }
        );

            endpoints.MapControllerRoute(
            name: "PermissionReport",
            pattern: "IzinRaporlari",
            defaults: new { controller = "PermissionReport", action = "PermissionReport" }
        );


            endpoints.MapControllerRoute(
            name: "VehicleRequestReport",
            pattern: "AracRaporlari",
            defaults: new { controller = "VehicleReport", action = "VehicleRequestReport" }
        );

            endpoints.MapControllerRoute(
            name: "MyVehicleRequests",
            pattern: "AracTaleplerim",
            defaults: new { controller = "VehicleRequest", action = "MyVehicleRequests" }
        );

            endpoints.MapControllerRoute(
            name: "VehicleRequests",
            pattern: "AracTalepleri",
            defaults: new { controller = "VehicleRequest", action = "VehicleRequest" }
        );

            endpoints.MapControllerRoute(
            name: "EmployeeCalendar",
            pattern: "PersonelTakvimi",
            defaults: new { controller = "EmployeeCalendar", action = "EmployeeCalendar" }
        );

            endpoints.MapControllerRoute(
            name: "Dashboard",
            pattern: "Anasayfa",
            defaults: new { controller = "Home", action = "Index" }
        );

            endpoints.MapControllerRoute(
             name: "AllMissions",
             pattern: "TumGorevler",
             defaults: new { controller = "AllMissions", action = "AllMissions" }
         );

            endpoints.MapControllerRoute(
              name: "CarRequests",
              pattern: "AracTakvimi",
              defaults: new { controller = "VehicleCalendar", action = "VehicleCalendar" }
          );

            endpoints.MapControllerRoute(
               name: "MyMissions",
               pattern: "Gorevlerim",
               defaults: new { controller = "Mission", action = "Mission" }
           );

            endpoints.MapControllerRoute(
               name: "Vehicles",
               pattern: "Araclar",
               defaults: new { controller = "Vehicle", action = "Vehicle" }
           );

            endpoints.MapControllerRoute(
               name: "AcademyInfos",
               pattern: "AkademikBilgilerim",
               defaults: new { controller = "AcademyInfo", action = "AcademyInfo" }
           );

            endpoints.MapControllerRoute(
               name: "AccessDenied_Route",
               pattern: "YetkiYok",
               defaults: new { controller = "Error", action = "AccessDenied" }
           );

            endpoints.MapControllerRoute(
                name: "AllPermissions",
                pattern: "Izinler",
                defaults: new { controller = "AllPermissions", action = "AllPermissions" }
            );

            endpoints.MapControllerRoute(
                name: "Permission",
                pattern: "Izinlerim",
                defaults: new { controller = "Permission", action = "Permission" }
            );

            endpoints.MapControllerRoute(
               name: "DepartmentStaff",
               pattern: "BirimPersonelleri",
               defaults: new { controller = "DepartmentStaff", action = "DepartmentStaff" }
           );

            endpoints.MapControllerRoute(
               name: "Address",
               pattern: "Adreslerim",
               defaults: new { controller = "Address", action = "Address" }
           );

            endpoints.MapControllerRoute(
               name: "GSM",
               pattern: "Numaralarim",
               defaults: new { controller = "GSM", action = "GSM" }
           );

            endpoints.MapControllerRoute(
                name: "EMail",
                pattern: "EmailAdreslerim",
                defaults: new { controller = "EMail", action = "EMail" }
            );

            endpoints.MapControllerRoute(
               name: "Profile",
               pattern: "Profil",
               defaults: new { controller = "Profile", action = "Profile" }
           );

            endpoints.MapControllerRoute(
                name: "Employee",
                pattern: "Personeller",
                defaults: new { controller = "Employee", action = "Employee" }
            );

            endpoints.MapControllerRoute(
               name: "EducationalInstutions",
               pattern: "Kurumlar",
               defaults: new { controller = "EducationalInstitution", action = "EducationalInstitution" }
           );

            endpoints.MapControllerRoute(
                name: "Departments",
                pattern: "Birimler",
                defaults: new { controller = "Department", action = "Department" }
            );

            endpoints.MapControllerRoute(
                name: "Exams",
                pattern: "Sinavlar",
                defaults: new { controller = "Exam", action = "Exam" }
            );

            endpoints.MapControllerRoute(
                name: "Holidays",
                pattern: "TatilGunleri",
                defaults: new { controller = "Holiday", action = "PublicHoliday" }
            );

            endpoints.MapControllerRoute(
               name: "Mission_Type",
               pattern: "GorevTurleri",
               defaults: new { controller = "MissionType", action = "MissionType" }
           );

            endpoints.MapControllerRoute(
               name: "Apellations",
               pattern: "Unvanlar",
               defaults: new { controller = "Apellation", action = "Apellation" }
           );
        }
    }
}