using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class VehicleReadRepository : ReadRepository<Vehicle>, IVehicleReadRepository
    {
        private readonly DAContext _context;
        public VehicleReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

