using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class SaveDepartmentStaffDto
    {
        public Guid IdEmployeeFK { get; set; }
        public Guid IdApellationFK { get; set; }
    }
}