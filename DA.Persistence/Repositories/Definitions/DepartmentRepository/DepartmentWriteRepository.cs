using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class DepartmentWriteRepository : WriteRepository<Department>, IDepartmentWriteRepository
    {
        private readonly  DAContext _context;
        public DepartmentWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

