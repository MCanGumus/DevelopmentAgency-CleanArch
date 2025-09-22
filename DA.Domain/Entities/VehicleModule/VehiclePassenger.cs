using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class VehiclePassenger : BaseEntity
    {

        public Guid IdVehicleRequestFK { get; set; }
        public VehicleRequest VehicleRequest { get; set; }
        public Guid IdEmployeeFK { get; set; }
        public Employee Employee { get; set; }

        public bool IsDriver { get; set; }

    }
}
