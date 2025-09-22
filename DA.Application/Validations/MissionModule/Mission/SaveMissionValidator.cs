using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveMissionValidator : AbstractValidator<SaveMissionDto>
    {
        public SaveMissionValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();
            RuleFor(t => t.IdProxyFK).NotEmpty().NotNull();

            RuleFor(t => t.Area).NotEmpty().NotNull().MaximumLength(200);
            RuleFor(t => t.Subject).NotEmpty().NotNull().MaximumLength(500);
            RuleFor(t => t.IsAdvanceRequested).NotNull();
            RuleFor(t => t.AdvanceAmount);
            RuleFor(t => t.Notes).MaximumLength(500);
            RuleFor(t => t.DateOfStart).NotEmpty().NotNull();
            RuleFor(t => t.DateOfEnd).NotEmpty().NotNull();

        }

    }
}
