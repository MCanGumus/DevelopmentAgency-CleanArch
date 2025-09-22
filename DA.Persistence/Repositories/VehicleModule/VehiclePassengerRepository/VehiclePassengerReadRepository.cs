using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using DA.Persistence.Repositories;

namespace DA.Persistence.Repositories
{
    public class VehiclePassengerReadRepository : ReadRepository<VehiclePassenger>, IVehiclePassengerReadRepository
    {
        private readonly DAContext _context;
        public VehiclePassengerReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

