using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class EmployeeValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeValidator()
        {

            RuleFor(t => t.IdentificationNumber).NotEmpty().NotNull().MaximumLength(11);
            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(t => t.Surname).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(t => t.MotherName).NotEmpty().NotNull().MaximumLength(150);
            RuleFor(t => t.FatherName).NotEmpty().NotNull().MaximumLength(150);
            RuleFor(t => t.PlaceOfBirth).NotEmpty().NotNull().MaximumLength(200);
            RuleFor(t => t.DateOfBirth);
            RuleFor(t => t.DateOfStart).NotEmpty().NotNull();
            RuleFor(t => t.Email).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(t => t.Password).NotEmpty().NotNull().MaximumLength(256);
            RuleFor(t => t.PasswordSalt).NotEmpty().NotNull().MaximumLength(100);
            RuleFor(t => t.TotalYearlyLeave).GreaterThanOrEqualTo(0);
            RuleFor(t => t.TotalUnpaidLeave).GreaterThanOrEqualTo(0);
            RuleFor(t => t.TotalExcusedLeave).GreaterThanOrEqualTo(0);
            RuleFor(t => t.TotalEqualizationLeave).GreaterThanOrEqualTo(0);
            RuleFor(t => t.RegistrationNumber).MaximumLength(50);

        }

    }
}
