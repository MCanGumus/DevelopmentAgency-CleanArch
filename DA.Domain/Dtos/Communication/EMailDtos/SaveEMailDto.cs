using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class SaveEMailDto
    {
        public Guid IdEmployeeFK { get; set; }

        public string EMailAddress { get; set; } = string.Empty;

    }
}