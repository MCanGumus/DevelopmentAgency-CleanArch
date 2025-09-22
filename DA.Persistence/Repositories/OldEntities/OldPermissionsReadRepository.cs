using DA.Application.Repositories;
using DA.Application.Repositories.OldEntities;
using DA.Domain.Entities;
using DA.Domain.Entities.OldDatas;
using DA.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.Repositories.OldEntities
{
    public class OldPermissionsReadRepository : OldReadRepository<OldPermissions>, IOldPermissionsReadRepository
    {
        private readonly DAContext _context;
        public OldPermissionsReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}
