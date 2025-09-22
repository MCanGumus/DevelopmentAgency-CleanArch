using DA.Domain.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Application.Validations.Definition.EducationalInstitution
{
    public class UpdateEducationalInstitutionValidator : AbstractValidator<UpdateEducationalInstitutionDto>
    {
        public UpdateEducationalInstitutionValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(200);

        }

    }
}
