using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdatePermissionDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid IdEmployeeFK { get; set; }
        public Guid IdProxyFK { get; set; }
        public Guid IdDepartmentFK { get; set; }
        public Guid? IdDelegateFK { get; set; }

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