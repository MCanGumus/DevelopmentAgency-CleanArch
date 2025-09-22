using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class ApellationValidator : AbstractValidator<ApellationDto>
    {
        public ApellationValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);

        }

    }
}
