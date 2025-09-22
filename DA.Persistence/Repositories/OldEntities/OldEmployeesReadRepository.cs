using DA.Application.Repositories.OldEntities;
using DA.Domain.Entities.OldDatas;
using DA.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.Repositories.OldEntities
{
    public class OldEmployeesReadRepository : OldReadRepository<OldEmployees>, IOldEmployeesReadRepository
    {
        private readonly DAContext _context;
        public OldEmployeesReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}
