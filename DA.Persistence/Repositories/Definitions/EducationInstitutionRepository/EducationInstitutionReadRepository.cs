using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories.EducationalInstitutionRepository
{
    public class EducationalInstitutionReadRepository : ReadRepository<EducationalInstitution>, IEducationalInstitutionReadRepository
    {
        private readonly DAContext _context;
        public EducationalInstitutionReadRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

