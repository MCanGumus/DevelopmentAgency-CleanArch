using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class PermissionReadRepository : ReadRepository<Permission>, IPermissionReadRepository
    {
        private readonly  DAContext _context;
        public PermissionReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

