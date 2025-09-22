using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class SaveGSMNumberDto
    {
        public Guid IdEmployeeFK { get; set; }

        public string GSM { get; set; } = string.Empty;

    }
}