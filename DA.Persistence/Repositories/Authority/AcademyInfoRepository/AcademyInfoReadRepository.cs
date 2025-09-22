using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class AcademyInfoReadRepository : ReadRepository<AcademyInfo>, IAcademyInfoReadRepository
    {
        private readonly  DAContext _context;
        public AcademyInfoReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

