using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class VehicleValidator : AbstractValidator<VehicleDto>
    {
        public VehicleValidator()
        {

            RuleFor(t => t.Plate).NotEmpty().NotNull().MaximumLength(50);
     
        }

    }
}
