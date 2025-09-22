using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class UpdateVehiclePassengerDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid IdVehicleRequestFK { get; set; }
        public Guid IdEmployeeFK { get; set; }

        public bool IsDriver { get; set; }
    }
}