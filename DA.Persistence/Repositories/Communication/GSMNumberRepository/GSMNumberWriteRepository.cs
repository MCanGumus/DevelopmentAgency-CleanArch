using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class GSMNumberWriteRepository : WriteRepository<GSMNumber>, IGSMNumberWriteRepository
    {
        private readonly DAContext _context;
        public GSMNumberWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

