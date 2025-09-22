using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveVehicleValidator : AbstractValidator<SaveVehicleDto>
    {
        public SaveVehicleValidator()
        {

            RuleFor(t => t.Plate).NotEmpty().NotNull().MaximumLength(50);
        }

    }
}
