using HumanResource.API.Models;
namespace HumanResource.API.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetByManagerAsync(int managerId);
        Task<IEnumerable<Employee>> GetByJobAsync(string jobId);
        Task<IEnumerable<Employee>> GetByRoleAsync(int roleId);
        Task<IEnumerable<Employee>> SearchAsync(string keyword);
        Task<(IEnumerable<Employee> Employees, int TotalRecords)> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Employee>> GetMyTeamAsync(decimal managerId);
        Task<Employee?> GetHighestSalaryEmployeeAsync();
    }
}
