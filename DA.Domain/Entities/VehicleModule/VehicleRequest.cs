using DA.Domain.Entities;
using DA.Domain.Enums;

namespace DA.Domain.Entities
{
    public class VehicleRequest : BaseEntity
    {
        public Guid? IdEmployeeFK { get; set; }
        public Guid IdVehicleFK { get; set; }
        public Vehicle Vehicle { get; set; }
        public Guid? IdMissionFK { get; set; }
        public Mission? Mission { get; set; }

        public EnumRequestType RequestType { get; set; }
        public string? Description { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }
        public bool IsGoing { get; set; }

        public ICollection<VehiclePassenger>? VehiclePassengers { get; set; }


    }
}
