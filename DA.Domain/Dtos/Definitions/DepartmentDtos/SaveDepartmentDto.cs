using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class SaveDepartmentDto
    {

        public string Name { get; set; } = string.Empty;
        public Guid IdEmployeeFK { get; set; }
        public Guid IdBackupManager { get; set; }

    }
}