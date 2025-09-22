using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdateApellationDto : BaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}