using DA.Domain.Entities;
using DA.Domain.Enums;

namespace DA.Domain.Entities
{
    public class AcademyInfo : BaseEntity
    {

        public Guid IdEmployeeFK { get; set; }
        public Employee Employee { get; set; }

        public string University { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? ThesisTopic { get; set; }
        public EnumAcademyType AcademyType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }


    }
}
