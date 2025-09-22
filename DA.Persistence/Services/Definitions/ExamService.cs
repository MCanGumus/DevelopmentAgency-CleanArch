using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;

namespace DA.Persistence.Services
{
    public class ExamService: BaseService<Exam, UpdateExamDto, SaveExamDto, ExamDto>, IExamService
    {
        private readonly IReadRepository<Exam> _readRepository;
        private readonly IWriteRepository<Exam> _writeRepository;

        private readonly IMapper _mapper;
        public ExamService(IReadRepository<Exam> readRepository, IWriteRepository<Exam> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }
    }
}
