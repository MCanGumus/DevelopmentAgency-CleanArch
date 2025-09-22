using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class MissionTypeReadRepository : ReadRepository<MissionType>, IMissionTypeReadRepository
    {
        private readonly  DAContext _context;
        public MissionTypeReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

