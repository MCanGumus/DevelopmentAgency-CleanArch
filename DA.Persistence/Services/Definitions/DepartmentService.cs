using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DA.Persistence.Services
{
    public class DepartmentService: BaseService<Department, UpdateDepartmentDto, SaveDepartmentDto, DepartmentDto>, IDepartmentService
    {
        private readonly IReadRepository<Department> _readRepository;
        private readonly IWriteRepository<Department> _writeRepository;

        private readonly IMapper _mapper;
        public DepartmentService(IReadRepository<Department> readRepository, IWriteRepository<Department> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public List<Department> GetAllDepartmentWithEmployee()
        {
            return _readRepository.GetWhere(x => x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).ToList();
        }

        public Department GetDepartmentWithEmployee(Guid id)
        {
            return _readRepository.GetWhere(x => x.Id == id).Include(x => x.Employee).SingleOrDefault();
        }

        public Department GetHelperOfDepartment(Guid idEmployee)
        {
            return _readRepository.GetWhere(x => x.IdBackupManager == idEmployee).Include(x => x.Employee).FirstOrDefault();
        }

        public Department GetPresidentOfDepartment(Guid idEmployee)
        {
            return _readRepository.GetWhere(x => x.IdEmployeeFK == idEmployee).Include(x => x.Employee).FirstOrDefault();
        }
    }
}
