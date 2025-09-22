using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class AddressWriteRepository : WriteRepository<Address>, IAddressWriteRepository
    {
        private readonly DAContext _context;
        public AddressWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

