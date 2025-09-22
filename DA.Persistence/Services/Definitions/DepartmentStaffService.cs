using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DA.Persistence.Services
{
    public class DepartmentStaffService : BaseService<DepartmentStaff, UpdateDepartmentStaffDto, SaveDepartmentStaffDto, DepartmentStaffDto>, IDepartmentStaffService
    {
        private readonly IReadRepository<DepartmentStaff> _readRepository;
        private readonly IWriteRepository<DepartmentStaff> _writeRepository;

        private readonly IMapper _mapper;
        public DepartmentStaffService(IReadRepository<DepartmentStaff> readRepository, IWriteRepository<DepartmentStaff> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _mapper = mapper;
        }

        public List<DepartmentStaffDto> GetAllDepartmentStaffs()
        {
            var result = _readRepository.GetWhere(x => x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).Include(x => x.Apellation);

            List<DepartmentStaffDto> lstResult = new List<DepartmentStaffDto>();

            foreach (var item in result)
                lstResult.Add(_mapper.Map<DepartmentStaffDto>(item));

            return lstResult;
        }

        public DepartmentStaff GetDepartmentStaffByEmployee(Guid idEmployee)
        {
            return _readRepository.GetWhere(x => x.IdEmployeeFK == idEmployee && x.DataType == Domain.Enums.EnumDataType.New).FirstOrDefault();
        }

        public DepartmentStaff GetDepartmentStaffById(Guid id)
        {
            return _readRepository.GetWhere(x => x.Id == id && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Employee).FirstOrDefault();
        }
    }
}
