using DA.Domain.Enums;

namespace DA.Domain.Dtos
{
    public class BaseDto
    {
        public DateTime RecordDate { get; set; }
        public EnumDataType DataType { get; set; }
    }
}
