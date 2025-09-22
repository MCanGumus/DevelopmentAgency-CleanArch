using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class VehicleDto : BaseDto
    {
        public Guid Id { get; set; }

        public string Plate { get; set; } = string.Empty;
        public bool IsTemporary { get; set; }
        public bool IsActive { get; set; }
        public int Capacity { get; set; }
    }
}