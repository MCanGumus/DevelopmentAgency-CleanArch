using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveMissionTypeValidator : AbstractValidator<SaveMissionTypeDto>
    {
        public SaveMissionTypeValidator()
        {

            RuleFor(t => t.TypeName).NotEmpty().NotNull().MaximumLength(50);

        }

    }
}
