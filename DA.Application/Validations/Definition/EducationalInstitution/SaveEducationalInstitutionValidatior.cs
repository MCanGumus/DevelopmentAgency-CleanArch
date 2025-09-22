using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveEducationalInstitutionValidator : AbstractValidator<SaveEducationalInstitutionDto>
    {
        public SaveEducationalInstitutionValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(200);

        }

    }
}
