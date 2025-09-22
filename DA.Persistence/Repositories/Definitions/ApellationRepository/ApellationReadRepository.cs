using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class ApellationReadRepository : ReadRepository<Apellation>, IApellationReadRepository
    {
        private readonly  DAContext _context;
        public ApellationReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

