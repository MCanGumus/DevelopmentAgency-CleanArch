using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class DepartmentValidator : AbstractValidator<DepartmentDto>
    {
        public DepartmentValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);

        }

    }
}
