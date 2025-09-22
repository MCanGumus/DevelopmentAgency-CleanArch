using AutoMapper;
using DA.Application.Abstractions;
using DA.Application.Abstractions.Authority;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Dtos.Authority.FamilyMemberDtos;
using DA.Domain.Entities;
using DA.Domain.Entities.Authority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.Services.Authority
{
    public class FamilyMemberService : BaseService<FamilyMember, UpdateFamilyMemberDto, SaveFamilyMemberDto, FamilyMemberDto>, IFamilyMemberService
    {
        private readonly IReadRepository<FamilyMember> _readRepository;
        private readonly IWriteRepository<FamilyMember> _writeRepository;

        private readonly IMapper _mapper;
        public FamilyMemberService(IReadRepository<FamilyMember> readRepository, IWriteRepository<FamilyMember> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _mapper = mapper;
        }

        public List<FamilyMemberDto> GetFamilyMembers(Guid idEmployee)
        {
            var list = _readRepository.GetWhere(x => x.IdEmployeeFK == idEmployee && x.DataType == Domain.Enums.EnumDataType.New).ToList();

            List<FamilyMemberDto> dtoList = _mapper.Map<List<FamilyMember>, List<FamilyMemberDto>>(list);

            return dtoList;
        }
    }
}
