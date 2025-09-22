using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class  UpdatePublicHolidayValidator : AbstractValidator<UpdatePublicHolidayDto>
    {
        public UpdatePublicHolidayValidator()
        {
            RuleFor(t => t.Date).NotEmpty().NotNull();

        }
        
    }
}
