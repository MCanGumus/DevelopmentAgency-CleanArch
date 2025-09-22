using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class EmployeeDto : BaseDto
    {
        public Guid Id { get; set; }
        public DepartmentDto Department { get; set; }

        public string IdentificationNumber { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Apellation { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string PlaceOfBirth { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfStart { get; set; }
        public EnumBloodGroup? BloodGroup { get; set; }
        public EnumGender Gender { get; set; }
        public EnumAuthorizationStatus AuthorizationStatus { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public int RefresherYearlyLeave { get; set; }
        public int TotalYearlyLeave { get; set; }
        public int TotalUnpaidLeave { get; set; }
        public int TotalExcusedLeave { get; set; }
        public int TotalEqualizationLeave { get; set; }
        public string? RegistrationNumber { get; set; }
        public Guid? Chief { get; set; }
        public DateTime? ExitedDate { get; set; }

    }
}