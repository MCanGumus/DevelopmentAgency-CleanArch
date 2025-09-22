using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class EMail : BaseEntity
    {

        public Guid IdEmployeeFK { get; set; }
        public Employee Employee { get; set; }

        public string EMailAddress { get; set; } = string.Empty;


    }
}
