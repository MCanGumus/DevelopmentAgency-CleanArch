using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdatePublicHolidayDto : BaseDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public bool IsNationalHoliday { get; set; }
    }
}