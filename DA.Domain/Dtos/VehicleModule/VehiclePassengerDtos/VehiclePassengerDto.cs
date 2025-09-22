using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class VehiclePassengerDto : BaseDto
    {
        public Guid Id { get; set; }
        public VehicleRequestDto VehicleRequest { get; set; }
        public EmployeeDto Employee { get; set; }

        public bool IsDriver { get; set; }
    }
}