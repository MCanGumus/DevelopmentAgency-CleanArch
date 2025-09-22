using DA.Domain.Entities;
using DA.Domain.Entities.Authority;
using DA.Domain.Enums;

namespace DA.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public Guid? IdDepartmentFK { get; set; }
        public Department? Department { get; set; }

        public string IdentificationNumber { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Apellation { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string PlaceOfBirth { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateOfStart { get; set; }
        public EnumBloodGroup? BloodGroup { get; set; }
        public EnumGender Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public EnumAuthorizationStatus AuthorizationStatus { get; set; }
        public int RefresherYearlyLeave { get; set; }
        public int TotalYearlyLeave { get; set; }
        public int TotalUnpaidLeave { get; set; }
        public int TotalExcusedLeave { get; set; }
        public int TotalEqualizationLeave { get; set; }
        public string? RegistrationNumber { get; set; }
        public Guid? Chief { get; set; }
        public DateTime? ExitedDate { get; set; }

        public ICollection<Address>? Addresses { get; set; }
        public ICollection<GSMNumber>? GSMNumbers { get; set; }
        public ICollection<EMail>? EMails { get; set; }
        public ICollection<Department>? Departments{ get; set; }
        public ICollection<DepartmentStaff> DepartmentStaffs { get; set; }
        public ICollection<Permission> Permissions{ get; set; }
        public ICollection<Permission> PermissionDelegates { get; set; }
        public ICollection<Permission> PermissionProxies { get; set; }
        public ICollection<AcademyInfo> AcademyInfos{ get; set; }
        public ICollection<Mission>? Missions { get; set; }
        public ICollection<Mission>? Proxies { get; set; }
        public ICollection<Mission>? WhoAccepts { get; set; }
        public ICollection<VehiclePassenger>? VehiclePassengers { get; set; }
        public ICollection<FamilyMember>? FamilyMembers { get; set; }

    }
}
