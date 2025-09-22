using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class EMailWriteRepository : WriteRepository<EMail>, IEMailWriteRepository
    {
        private readonly DAContext _context;
        public EMailWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

