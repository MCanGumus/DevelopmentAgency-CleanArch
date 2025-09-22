using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class MissionTypeWriteRepository : WriteRepository<MissionType>, IMissionTypeWriteRepository
    {
        private readonly  DAContext _context;
        public MissionTypeWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

