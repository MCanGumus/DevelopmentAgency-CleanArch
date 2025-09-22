using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveDepartmentStaffValidator : AbstractValidator<SaveDepartmentStaffDto>
    {
        public SaveDepartmentStaffValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();


        }

    }
}
