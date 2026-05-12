using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HRDbContext _context;

        public AuthRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Email == email);
        }
        public async Task AddUserAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);

            await _context.SaveChangesAsync();
        }
        public async Task<Employee?> GetLastEmployeeAsync()
        {
            return await _context.Employees
                .OrderByDescending(e => e.EmployeeId)
                .FirstOrDefaultAsync();
        }
    }
}