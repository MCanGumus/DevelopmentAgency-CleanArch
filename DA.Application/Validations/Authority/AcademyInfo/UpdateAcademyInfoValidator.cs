using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateAcademyInfoValidator : AbstractValidator<UpdateAcademyInfoDto>
    {
        public UpdateAcademyInfoValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();

            RuleFor(t => t.University).NotEmpty().NotNull().MaximumLength(200);
            RuleFor(t => t.Faculty).NotEmpty().NotNull().MaximumLength(200);
            RuleFor(t => t.Department).NotEmpty().NotNull().MaximumLength(200);
            RuleFor(t => t.ThesisTopic).MaximumLength(200);
            RuleFor(t => t.StartDate).NotEmpty().NotNull();
            RuleFor(t => t.EndDate);

        }

    }
}
