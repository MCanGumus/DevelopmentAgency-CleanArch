using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class ExamReadRepository : ReadRepository<Exam>, IExamReadRepository
    {
        private readonly  DAContext _context;
        public ExamReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

