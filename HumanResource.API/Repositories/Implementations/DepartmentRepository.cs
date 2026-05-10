using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Repositories.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly HRDbContext _context;

        public DepartmentRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Location)
                .Include(d => d.Manager)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(decimal id)
        {
            return await _context.Departments
                .Include(d => d.Location)
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<IEnumerable<Department>> GetByLocationAsync(decimal locationId)
        {
            return await _context.Departments
                .Include(d => d.Location)
                .Include(d => d.Manager)
                .Where(d => d.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<Department> AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);

            await _context.SaveChangesAsync();

            return department;
        }

        public async Task<Department> UpdateAsync(Department department)
        {
            _context.Departments.Update(department);

            await _context.SaveChangesAsync();

            return department;
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
                return false;

            _context.Departments.Remove(department);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}