using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;

namespace DA.Persistence.Repositories.EducationalInstitutionRepository
{
    public class EducationalInstitutionWriteRepository : WriteRepository<EducationalInstitution>, IEducationalInstitutionWriteRepository
    {
        private readonly DAContext _context;
        public EducationalInstitutionWriteRepository(DAContext context) : base(context)
        {
            _context = context;
        }
    }
}

