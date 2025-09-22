using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class VehicleRequestValidator : AbstractValidator<VehicleRequestDto>
    {
        public VehicleRequestValidator()
        {

            RuleFor(t => t.Description).MaximumLength(500);
            RuleFor(t => t.DateOfStart).NotEmpty().NotNull();
            RuleFor(t => t.DateOfEnd).NotEmpty().NotNull();

        }

    }
}

