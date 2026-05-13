using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Repositories.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HRDbContext _context;

        public EmployeeRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee != null)
            {
                await _context.Entry(employee)
                    .Reference(e => e.Role)
                    .LoadAsync();

                await _context.Entry(employee)
                    .Reference(e => e.Department)
                    .LoadAsync();

                await _context.Entry(employee)
                    .Reference(e => e.Manager)
                    .LoadAsync();
            }

            return employee;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.Employees
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByManagerAsync(int managerId)
        {
            return await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByJobAsync(string jobId)
        {
            return await _context.Employees
                .Where(e => e.JobId == jobId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByRoleAsync(int roleId)
        {
            return await _context.Employees
                .Where(e => e.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchAsync(string keyword)
        {
            return await _context.Employees
                .Where(e =>
                    e.FirstName.Contains(keyword) ||
                    e.LastName.Contains(keyword) ||
                    e.Email.Contains(keyword))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Employee> Employees, int TotalRecords)>
            GetPaginatedAsync(int pageNumber, int pageSize)
        {
            var totalRecords =
                await _context.Employees.CountAsync();

            var employees = await _context.Employees
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalRecords);
        }
        public async Task<IEnumerable<Employee>> GetMyTeamAsync(decimal managerId)
        {
            var employees = await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();

            foreach (var employee in employees)
            {
                await _context.Entry(employee)
                    .Reference(e => e.Department)
                    .LoadAsync();
            }

            return employees;
        }
        public async Task<Employee> GetHighestSalaryEmployeeAsync()
        {
            return await _context.Employees.OrderByDescending(e => e.Salary).FirstOrDefaultAsync();
        }
    }
}