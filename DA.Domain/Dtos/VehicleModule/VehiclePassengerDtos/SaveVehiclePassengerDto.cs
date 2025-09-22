using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class SaveVehiclePassengerDto
    {
        public Guid IdVehicleRequestFK { get; set; }
        public Guid IdEmployeeFK { get; set; }

        public bool IsDriver { get; set; }
    }
}