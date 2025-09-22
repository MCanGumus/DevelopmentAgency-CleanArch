using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class DepartmentStaffWriteRepository : WriteRepository<DepartmentStaff>, IDepartmentStaffWriteRepository
    {
        private readonly  DAContext _context;
        public DepartmentStaffWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

