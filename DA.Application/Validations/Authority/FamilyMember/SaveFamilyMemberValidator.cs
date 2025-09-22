using DA.Domain.Dtos.Authority.FamilyMemberDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Application.Validations.Authority.FamilyMember
{
    public class SaveFamilyMemberValidator : AbstractValidator<SaveFamilyMemberDto>
    {
        public SaveFamilyMemberValidator()
        {
            RuleFor(t => t.NationalIdentityNumber).NotEmpty().NotNull().MaximumLength(11);
            RuleFor(t => t.NameSurname).NotEmpty().NotNull().MaximumLength(70);
            RuleFor(t => t.MotherName).MaximumLength(150);
            RuleFor(t => t.FatherName).MaximumLength(150);
            RuleFor(t => t.SchoolName).MaximumLength(150);
            RuleFor(t => t.Class).MaximumLength(150);
            RuleFor(t => t.DateOfBirth);
        }
       
    }
}
