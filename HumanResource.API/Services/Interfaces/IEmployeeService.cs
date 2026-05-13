using HumanResource.API.DTOs;
using HumanResource.API.DTOs.Common;
namespace HumanResource.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<EmployeeDto>> GetAllAsync();
        public Task<EmployeeDto> GetByIdAsync(int id);
        public Task<EmployeeDto> AddAsync(EmployeeDto employeeDto);
        public Task<EmployeeDto> UpdateAsync(EmployeeDto employeeDto);
        public Task<bool> DeleteAsync(int id);
        public Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId);
        public Task<IEnumerable<EmployeeDto>> GetByManagerAsync(int managerId);
        public Task<IEnumerable<EmployeeDto>> GetByJobAsync(string jobId);
        public Task<IEnumerable<EmployeeDto>> GetByRoleAsync(int roleId);
        public Task<IEnumerable<EmployeeDto>> SearchAsync(string keyword);
        Task<PaginatedResponseDto<EmployeeDto>> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<MyTeamEmployeeDto>> GetMyTeamAsync(decimal managerId);
        public Task<EmployeeDto> GetHighestSalaryEmployeeAsync();
    }
}
