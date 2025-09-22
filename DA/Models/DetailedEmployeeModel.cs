using DA.Domain.Dtos;

namespace DA.Models
{
    public class DetailedEmployeeModel
    {
        public EmployeeDto Employee { get; set; }
        public List<AddressDto> Address { get; set; }
        public List<GSMNumberDto> GSMNumber { get; set; }
        public List<AcademyInfoDto> AcademyInfo{ get; set; }
        public List<EMailDto> Emails { get; set; }
    }
}
