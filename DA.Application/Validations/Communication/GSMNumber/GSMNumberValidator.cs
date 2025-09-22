using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class GSMNumberValidator : AbstractValidator<GSMNumberDto>
    {
        public GSMNumberValidator()
        {

            RuleFor(t => t.GSM).NotEmpty().NotNull().MaximumLength(20);

        }

    }
}
