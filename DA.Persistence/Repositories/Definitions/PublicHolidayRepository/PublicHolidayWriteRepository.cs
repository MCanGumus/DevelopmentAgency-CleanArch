using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class PublicHolidayWriteRepository : WriteRepository<PublicHoliday>, IPublicHolidayWriteRepository
    {
        private readonly  DAContext _context;
        public PublicHolidayWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

