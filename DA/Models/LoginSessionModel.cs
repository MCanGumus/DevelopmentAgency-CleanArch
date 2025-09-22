namespace DA.Models
{
    public class LoginSessionModel
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public bool ExtendedRole { get; set; }
        public string Photo { get; set; }
        public Guid UserGid { get; set; }
        public Guid DepartmentGid{ get; set; }
        public Guid? DepartmentHeadGid { get; set; }
        public Guid? DepartmentHelperGid { get; set; }
        public List<Guid> DepartmentAppointments { get; set; }
    }
}
