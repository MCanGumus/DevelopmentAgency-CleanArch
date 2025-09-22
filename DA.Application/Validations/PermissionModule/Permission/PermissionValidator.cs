using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class PermissionValidator : AbstractValidator<PermissionDto>
    {
        public PermissionValidator()
        {

            RuleFor(t => t.PermissionReason).NotEmpty().NotNull().MaximumLength(200);
            RuleFor(t => t.ExcusedLeave).MaximumLength(150);
            RuleFor(t => t.PermissionAddress).NotEmpty().NotNull().MaximumLength(300);
            RuleFor(t => t.StartDate).NotEmpty().NotNull();
            RuleFor(t => t.EndDate).NotEmpty().NotNull();
            RuleFor(t => t.Description).MaximumLength(500);

        }

    }
}
