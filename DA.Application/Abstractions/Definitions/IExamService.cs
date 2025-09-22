using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IExamService : IBaseService<Exam, UpdateExamDto, SaveExamDto, ExamDto>
    {
        
    }
}
