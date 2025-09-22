using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class PublicHolidayReadRepository : ReadRepository<PublicHoliday>, IPublicHolidayReadRepository
    {
        private readonly  DAContext _context;
        public PublicHolidayReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

