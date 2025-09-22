using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class VehiclePassengerValidator : AbstractValidator<VehiclePassengerDto>
    {
        public VehiclePassengerValidator()
        {

            RuleFor(t => t.IsDriver).NotEmpty().NotNull();

        }

    }
}
