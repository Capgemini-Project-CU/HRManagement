using HumanResource.API.DTOs;

namespace HumanResource.API.Services.Interfaces
{
    public interface IJobHistoryService
    {
        Task<IEnumerable<JobHistoryDto>> GetAllAsync();

        Task<JobHistoryDto> GetByIdAsync(int employeeId);

        Task<JobHistoryDto> AddAsync(JobHistoryDto jobHistoryDto);

        Task<bool> DeleteAsync(int employeeId);
        Task<IEnumerable<JobHistoryDto>> GetByDepartmentAsync(int departmentId);
    }
}
