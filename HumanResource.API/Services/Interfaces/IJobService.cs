using HumanResource.API.DTOs;

namespace HumanResource.API.Services.Interfaces
{
    public interface IJobService
    {
        Task<IEnumerable<JobDto>> GetAllAsync();
        Task<IEnumerable<JobDto>> GetBySalaryRangeAsync(decimal min, decimal max);

        Task<JobDto> GetByIdAsync(string id);

        Task<JobDto> CreateAsync(JobDto dto);

        Task<bool> UpdateAsync(string id, JobDto dto);

        Task<bool> DeleteAsync(string id);
    }
}