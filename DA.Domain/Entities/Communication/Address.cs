using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class Address : BaseEntity
    {

        public Guid IdEmployeeFK { get; set; }
        public Employee Employee { get; set; }

        public string AddressTitle { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;


    }
}
