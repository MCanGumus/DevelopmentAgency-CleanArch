using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using System.Collections.Generic;

namespace DA.Persistence.Services
{
    public class EMailService : BaseService<EMail, UpdateEMailDto, SaveEMailDto, EMailDto>, IEMailService
    {
        private readonly IReadRepository<EMail> _readRepository;
        private readonly IWriteRepository<EMail> _writeRepository;

        private readonly IMapper _mapper;
        public EMailService(IReadRepository<EMail> readRepository, IWriteRepository<EMail> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _mapper = mapper;
        }

        public List<EMailDto> GetAllMyMails(Guid id)
        {
            var list = _readRepository.GetWhere(x => x.IdEmployeeFK == id && x.DataType == Domain.Enums.EnumDataType.New).ToList();

            List<EMailDto> lstEmail = new List<EMailDto>();

            foreach (var item in list)
                lstEmail.Add(_mapper.Map<EMailDto>(item));

            return lstEmail;
        }
    }
}
