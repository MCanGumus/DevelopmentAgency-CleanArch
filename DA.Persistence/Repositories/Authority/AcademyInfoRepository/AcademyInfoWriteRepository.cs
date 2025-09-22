using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class AcademyInfoWriteRepository : WriteRepository<AcademyInfo>, IAcademyInfoWriteRepository
    {
        private readonly DAContext _context;
        public AcademyInfoWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

