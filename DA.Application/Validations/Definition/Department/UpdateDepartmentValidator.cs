using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);

        }

    }
}
