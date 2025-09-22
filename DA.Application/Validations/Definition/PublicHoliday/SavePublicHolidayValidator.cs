using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class  SavePublicHolidayValidator : AbstractValidator<SavePublicHolidayDto>
    {
        public SavePublicHolidayValidator()
        {
            RuleFor(t => t.Date).NotEmpty().NotNull();

        }
        
    }
}
