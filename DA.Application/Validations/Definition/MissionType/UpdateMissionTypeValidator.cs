using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateMissionTypeValidator : AbstractValidator<UpdateMissionTypeDto>
    {
        public UpdateMissionTypeValidator()
        {

            RuleFor(t => t.TypeName).NotEmpty().NotNull().MaximumLength(50);

        }

    }
}
