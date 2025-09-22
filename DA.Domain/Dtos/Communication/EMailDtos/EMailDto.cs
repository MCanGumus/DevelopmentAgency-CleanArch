using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class EMailDto : BaseDto
    {
        public Guid Id { get; set; }
        public EmployeeDto Employee { get; set; }

        public string EMailAddress { get; set; } = string.Empty;

    }
}