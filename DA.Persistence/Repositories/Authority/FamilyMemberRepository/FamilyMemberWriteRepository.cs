using DA.Application.Repositories;
using DA.Domain.Entities.Authority;
using DA.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.Repositories.Authority.FamilyMemberRepository
{
    public class FamilyMemberWriteRepository : WriteRepository<FamilyMember>, IFamilyMemberWriteRepository
    {
        private readonly DAContext _context;
        public FamilyMemberWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    
    }
}
