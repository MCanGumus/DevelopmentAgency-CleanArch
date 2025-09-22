using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class EMailReadRepository : ReadRepository<EMail>, IEMailReadRepository
    {
        private readonly DAContext _context;
        public EMailReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

