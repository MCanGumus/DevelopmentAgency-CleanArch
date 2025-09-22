using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class MissionReadRepository : ReadRepository<Mission>, IMissionReadRepository
    {
        private readonly  DAContext _context;
        public MissionReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

