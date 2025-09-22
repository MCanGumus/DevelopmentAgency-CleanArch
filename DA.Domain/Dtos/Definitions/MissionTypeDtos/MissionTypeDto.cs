using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class MissionTypeDto : BaseDto
    {
        public Guid Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

    }
}