using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class VehiclePassengerWriteRepository : WriteRepository<VehiclePassenger>, IVehiclePassengerWriteRepository
    {
        private readonly DAContext _context;
        public VehiclePassengerWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

