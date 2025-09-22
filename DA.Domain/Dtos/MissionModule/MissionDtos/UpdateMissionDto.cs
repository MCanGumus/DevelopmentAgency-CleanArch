using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdateMissionDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid IdEmployeeFK { get; set; }
        public Guid IdProxyFK { get; set; }
        public Guid? IdWhoAcceptedFK { get; set; }

        public string DocumentId { get; set; }
        public EnumMissionType MissionType { get; set; }
        public string Area { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public bool IsAdvanceRequested { get; set; }
        public long? AdvanceAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }
        public EnumDepartureVehicle DepartureVehicle { get; set; }
        public EnumReturnVehicle ReturnVehicle { get; set; }
        public EnumState State { get; set; }
        public EnumMissionSubjectType SubjectType { get; set; }

    }
}