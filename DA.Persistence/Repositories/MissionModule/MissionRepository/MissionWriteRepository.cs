using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class MissionWriteRepository : WriteRepository<Mission>, IMissionWriteRepository
    {
        private readonly DAContext _context;
        public MissionWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

