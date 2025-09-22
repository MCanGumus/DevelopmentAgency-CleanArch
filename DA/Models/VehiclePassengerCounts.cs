namespace DA.Models
{
    public class VehiclePassengerCounts
    {
        public Guid IdRequestFK { get; set; }
        public string Plate{ get; set; }
        public string CountPercentage{ get; set; }
        public bool IsGoing { get; set; }
    }
}
