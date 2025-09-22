using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class EmployeeWriteRepository : WriteRepository<Employee>, IEmployeeWriteRepository
    {
        private readonly  DAContext _context;
        public EmployeeWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

