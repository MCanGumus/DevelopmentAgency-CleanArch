using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class DepartmentReadRepository : ReadRepository<Department>, IDepartmentReadRepository
    {
        private readonly  DAContext _context;
        public DepartmentReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

