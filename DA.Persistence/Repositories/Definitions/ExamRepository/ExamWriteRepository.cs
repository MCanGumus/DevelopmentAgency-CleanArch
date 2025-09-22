using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class ExamWriteRepository : WriteRepository<Exam>, IExamWriteRepository
    {
        private readonly  DAContext _context;
        public ExamWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

