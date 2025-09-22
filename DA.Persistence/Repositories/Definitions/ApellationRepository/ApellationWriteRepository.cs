using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class ApellationWriteRepository : WriteRepository<Apellation>, IApellationWriteRepository
    {
        private readonly  DAContext _context;
        public ApellationWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

