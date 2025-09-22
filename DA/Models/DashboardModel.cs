namespace DA.Models
{
    public class DashboardModel
    {
        public int RemainingFreePermission {  get; set; }
        public int RemainingPaidPermission { get; set; }
        public int RemainingExcusedPermission { get; set; }
        public int RemainingEqualizationPermission { get; set; }

        public List<DashboardCarsModel> Cars { get; set; }
    }
}

