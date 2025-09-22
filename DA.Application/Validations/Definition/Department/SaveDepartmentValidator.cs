using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveDepartmentValidator : AbstractValidator<SaveDepartmentDto>
    {
        public SaveDepartmentValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);
        }

    }
}
