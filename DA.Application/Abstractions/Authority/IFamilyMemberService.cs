using DA.Domain.Dtos;
using DA.Domain.Dtos.Authority.FamilyMemberDtos;
using DA.Domain.Entities;
using DA.Domain.Entities.Authority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Application.Abstractions.Authority
{
    public interface IFamilyMemberService : IBaseService<FamilyMember, UpdateFamilyMemberDto, SaveFamilyMemberDto, FamilyMemberDto>
    {
        List<FamilyMemberDto> GetFamilyMembers(Guid idEmployee);
    }
}
