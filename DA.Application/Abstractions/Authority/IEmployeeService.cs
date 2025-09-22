using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IEmployeeService : IBaseService<Employee, UpdateEmployeeDto, SaveEmployeeDto, EmployeeDto>
    {
        bool CheckRegistrationNumber(int number);
        List<EmployeeDto> GetAllEmployeesWithDepartment();

        EmployeeDto GetEmployeeWithMail(string mail);

        List<EmployeeDto> GetAllEmployeesExited();
    }
}
