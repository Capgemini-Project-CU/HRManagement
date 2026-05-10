using HumanResource.API.Models;

namespace HumanResource.API.Repositories.Interfaces
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllAsync();
        Task<IEnumerable<Job>> GetBySalaryRangeAsync(decimal min, decimal max);

        Task<Job?> GetByIdAsync(string id);

        Task AddAsync(Job job);

        Task UpdateAsync(Job job);

        Task DeleteAsync(Job job);
    }
}