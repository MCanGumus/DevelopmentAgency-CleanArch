using FluentValidation;
using DA.Domain.Dtos;

namespace DA.Application.Validation
{
    public class  ExamValidator : AbstractValidator<ExamDto>
    {
        public ExamValidator()
        {
            RuleFor(t => t.Name).NotEmpty().NotNull().MaximumLength(100);

        }
        
    }
}
