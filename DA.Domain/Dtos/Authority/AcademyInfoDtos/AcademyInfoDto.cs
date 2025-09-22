using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class AcademyInfoDto : BaseDto
    {
        public Guid Id { get; set; }
        public EmployeeDto Employee { get; set; }

        public string University { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? ThesisTopic { get; set; }
        public EnumAcademyType AcademyType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}