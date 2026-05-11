using HumanResource.API.Models;

namespace HumanResource.API.Repositories.Interfaces
{
    public interface IJobHistoryRepository
    {
        Task<IEnumerable<JobHistory>> GetAllAsync();
        Task<JobHistory> GetByIdAsync(int employeeId);
        Task<JobHistory> AddAsync(JobHistory jobHistory);
        Task<bool> DeleteAsync(int employeeId);
        Task<IEnumerable<JobHistory>> GetByDepartmentAsync(int departmentId);
    }
}