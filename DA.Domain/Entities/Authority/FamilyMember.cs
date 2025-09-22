using DA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Domain.Entities.Authority
{
    public class FamilyMember : BaseEntity
    {
        public Guid IdEmployeeFK { get; set; } 

        public string NameSurname { get; set; }
        public string NationalIdentityNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        
        public EnumRelationType RelationType { get; set; } 

        public bool? IsWorking { get; set; }
        public bool? HasIncome { get; set; }

       
        public EnumGender? Gender { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public EnumChildState? ChildState { get; set; }
        public bool? IsInEducation { get; set; }
        public DateTime? DateOfStart { get; set; }
        public string SchoolName { get; set; }
        public string Class { get; set; }

        public string Description { get; set; }

        public Employee Employee { get; set; }
    }
}
