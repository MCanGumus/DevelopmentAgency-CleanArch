using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class SaveAddressDto
    {
        public Guid IdEmployeeFK { get; set; }

        public string AddressTitle { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;

    }
}