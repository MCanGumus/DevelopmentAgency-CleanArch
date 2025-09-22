using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class PermissionWriteRepository : WriteRepository<Permission>, IPermissionWriteRepository
    {
        private readonly DAContext _context;
        public PermissionWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

