using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class PublicHoliday : BaseEntity
    {


        public DateTime Date { get; set; }

        public bool IsNationalHoliday {  get; set; }
    }
}
