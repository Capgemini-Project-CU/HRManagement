using HumanResource.API.Models;

namespace HumanResource.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Employee?> GetByEmailAsync(string email);
        Task AddUserAsync(Employee employee);

        Task<Employee?> GetLastEmployeeAsync();
    }
}