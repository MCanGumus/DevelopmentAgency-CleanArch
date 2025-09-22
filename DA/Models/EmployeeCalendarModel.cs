using DA.Domain.Dtos;

namespace DA.Models
{
    public class EmployeeCalendarModel
    {
        public DateTime Filter { get; set; }
        public Dictionary<DateTime, List<EmployeeCalendarModelPart>> Employees { get; set; } = new Dictionary<DateTime, List<EmployeeCalendarModelPart>>();
    }
}
