using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class PublicHolidayValidator : AbstractValidator<PublicHolidayDto>
    {
        public PublicHolidayValidator()
        {

            RuleFor(t => t.Date).NotEmpty().NotNull();

        }

    }
}
