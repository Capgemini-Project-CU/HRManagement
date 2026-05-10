using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _service;

        public JobsController(IJobService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(JobDto dto)
        {
            var createdJob = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdJob.JobId },
                createdJob);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, JobDto dto)
        {
            await _service.UpdateAsync(id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("salary-range")]
        public async Task<IActionResult> GetBySalaryRange(
            [FromQuery] decimal min,
            [FromQuery] decimal max)
        {
            var jobs = await _service.GetBySalaryRangeAsync(min, max);

            return Ok(jobs);
        }
    }
}