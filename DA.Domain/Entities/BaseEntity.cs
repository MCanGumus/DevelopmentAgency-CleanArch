using DA.Domain.Enums;

namespace DA.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id{ get; set; }
        public DateTime RecordDate { get; set; }
        public EnumDataType DataType { get; set; }
    }
}
