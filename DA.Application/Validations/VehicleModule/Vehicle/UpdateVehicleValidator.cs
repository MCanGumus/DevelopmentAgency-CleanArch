using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleDto>
    {
        public UpdateVehicleValidator()
        {

            RuleFor(t => t.Plate).NotEmpty().NotNull().MaximumLength(50);
  
        }

    }
}
