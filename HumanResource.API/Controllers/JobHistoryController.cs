using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobHistoryController : ControllerBase
    {
        private readonly IJobHistoryService _jobHistoryService;

        public JobHistoryController(IJobHistoryService jobHistoryService)
        {
            _jobHistoryService = jobHistoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR,Employee")]
        public async Task<IActionResult> GetAllJobHistory()
        {
            var jobHistory = await _jobHistoryService.GetAllAsync();

            return Ok(jobHistory);
        }

        [HttpGet("{employeeId}")]
        [Authorize(Roles = "Admin,HR,Employee")]
        public async Task<IActionResult> GetJobHistoryById(int employeeId)
        {
            var jobHistory = await _jobHistoryService.GetByIdAsync(employeeId);

            return Ok(jobHistory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> AddJobHistory(
            JobHistoryDto jobHistoryDto)
        {
            var createdJobHistory =
                await _jobHistoryService.AddAsync(jobHistoryDto);

            return Ok(createdJobHistory);
        }

        [HttpDelete("{employeeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteJobHistory(int employeeId)
        {
            await _jobHistoryService.DeleteAsync(employeeId);

            return Ok(
                $"Job History for Employee Id {employeeId} deleted successfully"
            );
        }

        [HttpGet("department/{departmentId}")]
        [Authorize(Roles = "Admin,HR,Employee")]
        public async Task<IActionResult> GetJobHistoryByDepartment(
            int departmentId)
        {
            var jobHistories =
                await _jobHistoryService.GetByDepartmentAsync(departmentId);

            return Ok(jobHistories);
        }
    }
}