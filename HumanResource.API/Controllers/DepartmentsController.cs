using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentsController(IDepartmentService service)
        {
            _service = service;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        // GET: api/departments/10
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Department not found");

            return Ok(result);
        }

        // GET: api/departments/location/1700
        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetByLocation(decimal locationId)
        {
            var result = await _service.GetByLocationAsync(locationId);

            return Ok(result);
        }

        // POST: api/departments
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDto dto)
        {
            var result = await _service.AddAsync(dto);

            return Ok(result);
        }

        // PUT: api/departments/10
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(decimal id, DepartmentDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result == null)
                return NotFound("Department not found");

            return Ok(result);
        }

        // DELETE: api/departments/10
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Department not found");

            return Ok("Department deleted successfully");
        }
    }
}