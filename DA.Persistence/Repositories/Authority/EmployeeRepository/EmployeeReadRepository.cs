using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class EmployeeReadRepository : ReadRepository<Employee>, IEmployeeReadRepository
    {
        private readonly  DAContext _context;
        public EmployeeReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

