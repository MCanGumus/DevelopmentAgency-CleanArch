namespace DA.Models
{
    public class EmployeeCalendarModelPart
    {
        public bool IsMission {  get; set; }
        public string Name { get; set; }    
        public string Description { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }
    }
}
