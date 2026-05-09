using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _service;

        public RolesController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _service.GetByIdAsync(id);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleDto dto)
        {
            var createdRole = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdRole.RoleId },
                createdRole);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RoleDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}