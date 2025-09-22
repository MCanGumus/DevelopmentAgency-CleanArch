using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IEducationalInstitutionService : IBaseService<EducationalInstitution, UpdateEducationalInstitutionDto, SaveEducationalInstitutionDto, EducationalInstitutionDto>
    {

    }
}
