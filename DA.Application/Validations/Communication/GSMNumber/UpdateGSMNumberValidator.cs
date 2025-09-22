using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateGSMNumberValidator : AbstractValidator<UpdateGSMNumberDto>
    {
        public UpdateGSMNumberValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();

            RuleFor(t => t.GSM).NotEmpty().NotNull().MaximumLength(20);

        }

    }
}
