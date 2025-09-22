using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class GSMNumber : BaseEntity
    {

        public Guid IdEmployeeFK { get; set; }
        public Employee Employee { get; set; }

        public string GSM { get; set; } = string.Empty;


    }
}
