using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class AddressValidator : AbstractValidator<AddressDto>
    {
        public AddressValidator()
        {

            RuleFor(t => t.AddressTitle).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(t => t.FullAddress).NotEmpty().NotNull().MaximumLength(250);

        }

    }
}
