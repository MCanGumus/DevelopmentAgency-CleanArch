using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveApellationValidator : AbstractValidator<SaveApellationDto>
    {
        public SaveApellationValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);

        }

    }
}
