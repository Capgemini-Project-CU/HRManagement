using Microsoft.AspNetCore.Mvc;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
namespace HumanResource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            return Ok(employee);
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeDto employeeDto)
        {
            var createdEmployee = await _employeeService.AddAsync(employeeDto);
            return Ok(createdEmployee);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employeeDto)
        {
            if (id != employeeDto.EmployeeId)
            {
                return BadRequest("Employee Id mismatch");
            }
            var updatedEmployee = await _employeeService.UpdateAsync(employeeDto);
            return Ok(updatedEmployee);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteAsync(id);
            return Ok($"Employee with Id {id} deleted successfully");
        }
        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetEmployeesByDepartment(int departmentId)
        {
            var employees = await _employeeService.GetByDepartmentAsync(departmentId);

            return Ok(employees);
        }

        [HttpGet("manager/{managerId}")]
        public async Task<IActionResult> GetEmployeesByManager(int managerId)
        {
            var employees = await _employeeService.GetByManagerAsync(managerId);

            return Ok(employees);
        }

        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetEmployeesByJob(string jobId)
        {
            var employees = await _employeeService.GetByJobAsync(jobId);
            return Ok(employees);
        }
        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetEmployeesByRole(int roleId)
        {
            var employees = await _employeeService.GetByRoleAsync(roleId);
            return Ok(employees);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees(string keyword)
        {
            var employees = await _employeeService.SearchAsync(keyword);
            return Ok(employees);
        }
        [HttpGet("pagination")]
        public async Task<IActionResult> GetPaginatedEmployees(int pageNumber, int pageSize)
        {
            var employees = await _employeeService.GetPaginatedAsync(pageNumber, pageSize);
            return Ok(employees);
        }
        [HttpGet("highest-salary")]
        public async Task<IActionResult> GetHighestSalaryEmployee()
        {
            var employee = await _employeeService.GetHighestSalaryEmployeeAsync();
            return Ok(employee);
        }
    }
}
