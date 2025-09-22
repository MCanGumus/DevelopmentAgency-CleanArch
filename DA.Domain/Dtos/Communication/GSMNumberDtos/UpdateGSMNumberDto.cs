using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdateGSMNumberDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid IdEmployeeFK { get; set; }

        public string GSM { get; set; } = string.Empty;

    }
}