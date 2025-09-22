using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdateDepartmentStaffDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid IdEmployeeFK { get; set; }
        public Guid IdApellationFK { get; set; }
    }
}