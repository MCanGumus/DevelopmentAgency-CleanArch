using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class VehicleWriteRepository : WriteRepository<Vehicle>, IVehicleWriteRepository
    {
        private readonly DAContext _context;
        public VehicleWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

