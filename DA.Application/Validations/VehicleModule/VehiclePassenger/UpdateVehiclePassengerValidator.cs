using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateVehiclePassengerValidator : AbstractValidator<UpdateVehiclePassengerDto>
    {
        public UpdateVehiclePassengerValidator()
        {
            RuleFor(t => t.IdVehicleRequestFK).NotEmpty().NotNull();
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();

            RuleFor(t => t.IsDriver).NotEmpty().NotNull();

        }

    }
}
