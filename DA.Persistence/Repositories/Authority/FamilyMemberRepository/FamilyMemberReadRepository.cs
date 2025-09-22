using DA.Application.Repositories;
using DA.Application.Repositories.Authority.FamilyMemberRepository;
using DA.Domain.Entities.Authority;
using DA.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.Repositories.Authority.FamilyMemberRepository
{
    internal class FamilyMemberReadRepository : ReadRepository<FamilyMember>, IFamilyMemberReadRepository
    {
        private readonly DAContext _context;
        public FamilyMemberReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    
    }
}
