using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class EMailValidator : AbstractValidator<EMailDto>
    {
        public EMailValidator()
        {

            RuleFor(t => t.EMailAddress).NotEmpty().NotNull().MaximumLength(50);

        }

    }
}
