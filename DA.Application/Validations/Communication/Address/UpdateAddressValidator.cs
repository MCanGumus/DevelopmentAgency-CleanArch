using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateAddressValidator : AbstractValidator<UpdateAddressDto>
    {
        public UpdateAddressValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();

            RuleFor(t => t.AddressTitle).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(t => t.FullAddress).NotEmpty().NotNull().MaximumLength(250);

        }

    }
}
