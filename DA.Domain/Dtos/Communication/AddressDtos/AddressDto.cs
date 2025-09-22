using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class AddressDto : BaseDto
    {
        public Guid Id { get; set; }
        public EmployeeDto Employee { get; set; }

        public string AddressTitle { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;

    }
}