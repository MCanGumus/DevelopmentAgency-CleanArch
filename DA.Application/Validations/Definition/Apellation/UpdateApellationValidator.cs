using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateApellationValidator : AbstractValidator<UpdateApellationDto>
    {
        public UpdateApellationValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);

        }

    }
}
