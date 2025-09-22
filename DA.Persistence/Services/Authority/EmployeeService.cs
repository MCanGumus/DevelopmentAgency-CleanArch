using AutoMapper;
using DA.Application.Repositories;
using DA.Domain.Dtos;
using DA.Domain.Entities;
using DA.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DA.Persistence.Services
{
    public class EmployeeService : BaseService<Employee, UpdateEmployeeDto, SaveEmployeeDto, EmployeeDto>, IEmployeeService
    {
        private readonly IReadRepository<Employee> _readRepository;
        private readonly IWriteRepository<Employee> _writeRepository;

        private readonly IMapper _mapper;
        public EmployeeService(IReadRepository<Employee> readRepository, IWriteRepository<Employee> writeRepository, IMapper mapper) : base(readRepository, writeRepository, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _mapper = mapper;
        }

        public bool CheckRegistrationNumber(int number)
        {
            var entity = _readRepository.GetWhere(x => x.RegistrationNumber == number.ToString()).FirstOrDefault();

            return entity == null ? false : true;
        }

        public List<EmployeeDto> GetAllEmployeesExited()
        {
            var employees = _readRepository.GetWhere(x => x.DataType == Domain.Enums.EnumDataType.Draft).Include(x => x.Department);

            List<EmployeeDto> employeeDto = new List<EmployeeDto>();

            foreach (var employee in employees)
                employeeDto.Add(_mapper.Map<EmployeeDto>(employee));

            return employeeDto;
        }

        public List<EmployeeDto> GetAllEmployeesWithDepartment()
        {
            var employees = _readRepository.GetWhere(x => x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Department).ToList();

            List <EmployeeDto> employeeDto = new List<EmployeeDto>();

            foreach (var employee in employees)
                employeeDto.Add(_mapper.Map<EmployeeDto>(employee));

            return employeeDto;
        }

        public EmployeeDto GetEmployeeWithMail(string mail)
        {
            var user = _readRepository.GetWhere(x => x.Email == mail && x.DataType == Domain.Enums.EnumDataType.New).Include(x => x.Department).FirstOrDefault();

            return _mapper.Map<EmployeeDto>(user);
        }
    }
}
