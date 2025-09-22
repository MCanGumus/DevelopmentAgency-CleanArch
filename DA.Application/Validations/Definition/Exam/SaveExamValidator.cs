using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class  SaveExamValidator : AbstractValidator<SaveExamDto>
    {
        public SaveExamValidator()
        {

            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);
        }
        
    }
}
