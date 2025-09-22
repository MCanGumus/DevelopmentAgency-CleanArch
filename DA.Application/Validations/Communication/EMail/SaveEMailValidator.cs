using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class SaveEMailValidator : AbstractValidator<SaveEMailDto>
    {
        public SaveEMailValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();

            RuleFor(t => t.EMailAddress).NotEmpty().NotNull().MaximumLength(50);

        }

    }
}
