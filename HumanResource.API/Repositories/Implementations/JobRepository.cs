using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Repositories.Implementations
{
    public class JobRepository : IJobRepository
    {
        private readonly HRDbContext _context;

        public JobRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _context.Jobs.ToListAsync();
        }
        public async Task<IEnumerable<Job>> GetBySalaryRangeAsync(decimal min, decimal max)
        {
            return await _context.Jobs
                .Where(j => j.MinSalary >= min && j.MaxSalary <= max)
                .ToListAsync();
        }
        public async Task<Job?> GetByIdAsync(string id)
        {
            return await _context.Jobs.FindAsync(id);
        }

        public async Task AddAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            _context.Jobs.Update(job);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Job job)
        {
            _context.Jobs.Remove(job);

            await _context.SaveChangesAsync();
        }
    }
}