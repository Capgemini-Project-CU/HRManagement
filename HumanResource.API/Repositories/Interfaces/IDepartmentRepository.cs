using HumanResource.API.Models;

namespace HumanResource.API.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(decimal id);

        Task<IEnumerable<Department>> GetByLocationAsync(decimal locationId);

        Task<Department> AddAsync(Department department);

        Task<Department> UpdateAsync(Department department);

        Task<bool> DeleteAsync(decimal id);
    }
}