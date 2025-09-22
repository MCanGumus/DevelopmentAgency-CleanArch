using DA.Domain.Entities;

namespace DA.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public string Plate { get; set; } = string.Empty;
        public bool IsTemporary { get; set; }
        public bool IsActive { get; set; }
        public int Capacity {  get; set; } 

        public ICollection<VehicleRequest>? VehicleRequests { get; set; }
        
    }
}
