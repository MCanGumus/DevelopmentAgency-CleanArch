using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class UpdateEMailValidator : AbstractValidator<UpdateEMailDto>
    {
        public UpdateEMailValidator()
        {
            RuleFor(t => t.IdEmployeeFK).NotEmpty().NotNull();

            RuleFor(t => t.EMailAddress).NotEmpty().NotNull().MaximumLength(50);

        }

    }
}
