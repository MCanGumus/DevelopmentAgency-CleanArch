using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class VehicleRequestDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid? IdEmployeeFK { get; set; }
        public VehicleDto Vehicle { get; set; }
        public MissionDto? Mission { get; set; }

        public EnumRequestType RequestType { get; set; }
        public string? Description { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }
        public bool IsGoing { get; set; }

    }
}