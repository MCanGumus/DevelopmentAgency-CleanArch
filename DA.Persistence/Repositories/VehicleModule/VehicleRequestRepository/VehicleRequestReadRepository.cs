using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class VehicleRequestReadRepository : ReadRepository<VehicleRequest>, IVehicleRequestReadRepository
    {
        private readonly DAContext _context;
        public VehicleRequestReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

