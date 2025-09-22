using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class MissionTypeValidator : AbstractValidator<MissionTypeDto>
    {
        public MissionTypeValidator()
        {

            RuleFor(t => t.TypeName).NotEmpty().NotNull().MaximumLength(50);

        }

    }
}
