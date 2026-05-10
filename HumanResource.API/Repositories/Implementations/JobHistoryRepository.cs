using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace HumanResource.API.Repositories.Implementations
{
    public class JobHistoryRepository : IJobHistoryRepository
    {
        public readonly HRDbContext _context;
        public JobHistoryRepository(HRDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<JobHistory>> GetAllAsync()
        {
            return await _context.JobHistories.ToListAsync();
        }
        public async Task<JobHistory> GetByIdAsync(int employeeId)
        {
            return await _context.JobHistories.FirstOrDefaultAsync(j => j.EmployeeId == employeeId);
        }
        public async Task<JobHistory> AddAsync(JobHistory jobHistory)
        {
            await _context.JobHistories.AddAsync(jobHistory);
            await _context.SaveChangesAsync();
            return jobHistory;
        }
        public async Task<bool> DeleteAsync(int employeeId)
        {
            var jobHistory = await _context.JobHistories.FirstOrDefaultAsync(j => j.EmployeeId == employeeId);
            if (jobHistory == null)
            {
                return false;
            }
            _context.JobHistories.Remove(jobHistory);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<JobHistory>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.JobHistories
                .Where(j => j.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
}
