using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdateVehicleRequestDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid? IdEmployeeFK { get; set; }
        public Guid IdVehicleFK { get; set; }
        public Guid IdMissionFK { get; set; }

        public EnumRequestType RequestType { get; set; }
        public string? Description { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }

        public bool IsGoing { get; set; }

    }
}