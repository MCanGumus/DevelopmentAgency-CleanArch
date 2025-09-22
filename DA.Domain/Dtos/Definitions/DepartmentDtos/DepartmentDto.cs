using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class DepartmentDto : BaseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public EmployeeDto Employee { get; set; }
        public Guid? IdBackupManager { get; set; }
    }
}