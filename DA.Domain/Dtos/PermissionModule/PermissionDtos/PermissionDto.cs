using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class PermissionDto : BaseDto
    {
        public Guid Id { get; set; }
        public EmployeeDto Employee { get; set; }
        public EmployeeDto Proxy { get; set; }
        public DepartmentDto Department { get; set; }
        public EmployeeDto? Delegate { get; set; }

        public string DocumentId { get; set; }
        public EnumPermissionType PermissionType { get; set; }
        public string PermissionReason { get; set; } = string.Empty;
        public string? ExcusedLeave { get; set; }
        public string PermissionAddress { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public EnumState State { get; set; }
        public string? RejectReason { get; set; }

    }
}