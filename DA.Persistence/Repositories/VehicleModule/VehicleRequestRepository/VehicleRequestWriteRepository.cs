using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories
{
    public class VehicleRequestWriteRepository : WriteRepository<VehicleRequest>, IVehicleRequestWriteRepository
    {
        private readonly DAContext _context;
        public VehicleRequestWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

