using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class  UpdateExamValidator : AbstractValidator<UpdateExamDto>
    {
        public UpdateExamValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);
        }
        
    }
}
