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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _service.GetByIdAsync(id);

            return Ok(result);
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetByLocation(decimal locationId)
        {
            var result = await _service.GetByLocationAsync(locationId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDto dto)
        {
            var result = await _service.AddAsync(dto);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(decimal id, DepartmentDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            var result = await _service.DeleteAsync(id);

            return Ok(result);
        }
    }
}