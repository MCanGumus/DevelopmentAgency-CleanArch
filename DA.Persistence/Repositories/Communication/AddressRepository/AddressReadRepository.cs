using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class AddressReadRepository : ReadRepository<Address>, IAddressReadRepository
    {
        private readonly  DAContext _context;
        public AddressReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

