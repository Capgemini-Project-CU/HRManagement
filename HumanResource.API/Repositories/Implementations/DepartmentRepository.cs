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
            var departments = await _context.Departments.ToListAsync();

            foreach (var department in departments)
            {
                await _context.Entry(department)
                    .Reference(d => d.Location)
                    .LoadAsync();

                await _context.Entry(department)
                    .Reference(d => d.Manager)
                    .LoadAsync();
            }

            return departments;
        }

        public async Task<Department?> GetByIdAsync(decimal id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department != null)
            {
                await _context.Entry(department)
                    .Reference(d => d.Location)
                    .LoadAsync();

                await _context.Entry(department)
                    .Reference(d => d.Manager)
                    .LoadAsync();
            }

            return department;
        }

        public async Task<IEnumerable<Department>> GetByLocationAsync(decimal locationId)
        {
            var departments = await _context.Departments
                .Where(d => d.LocationId == locationId)
                .ToListAsync();

            foreach (var department in departments)
            {
                await _context.Entry(department)
                    .Reference(d => d.Location)
                    .LoadAsync();

                await _context.Entry(department)
                    .Reference(d => d.Manager)
                    .LoadAsync();
            }

            return departments;
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
            var department = await _context.Departments
                .FindAsync(id);

            if (department == null)
                return false;

            _context.Departments.Remove(department);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}