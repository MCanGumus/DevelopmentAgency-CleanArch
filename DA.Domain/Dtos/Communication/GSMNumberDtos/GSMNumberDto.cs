using DA.Domain.Entities;
using DA.Domain.Enums;
using System.IO.Compression;

namespace DA.Domain.Dtos
{
    public class GSMNumberDto : BaseDto
    {
        public Guid Id { get; set; }
        public EmployeeDto Employee { get; set; }

        public string GSM { get; set; } = string.Empty;

    }
}