using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveVehicleRequestValidator : AbstractValidator<SaveVehicleRequestDto>
    {
        public SaveVehicleRequestValidator()
        {
            RuleFor(t => t.IdVehicleFK).NotEmpty().NotNull();
            RuleFor(t => t.IdMissionFK);

            RuleFor(t => t.Description).MaximumLength(500);
            RuleFor(t => t.DateOfStart).NotEmpty().NotNull();
            RuleFor(t => t.DateOfEnd).NotEmpty().NotNull();

        }

    }
}
