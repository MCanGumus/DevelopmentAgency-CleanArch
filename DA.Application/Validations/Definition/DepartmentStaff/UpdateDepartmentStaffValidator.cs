using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateDepartmentStaffValidator : AbstractValidator<UpdateDepartmentStaffDto>
    {
        public UpdateDepartmentStaffValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();


        }

    }
}
