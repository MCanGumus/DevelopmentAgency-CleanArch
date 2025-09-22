using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class GSMNumberReadRepository : ReadRepository<GSMNumber>, IGSMNumberReadRepository
    {
        private readonly DAContext _context;
        public GSMNumberReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

