using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class DepartmentStaffReadRepository : ReadRepository<DepartmentStaff>, IDepartmentStaffReadRepository
    {
        private readonly  DAContext _context;
        public DepartmentStaffReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

