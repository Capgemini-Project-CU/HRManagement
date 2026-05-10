using HumanResource.API.DTOs.LocationDto;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _service;

        public LocationsController(ILocationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet("country/{countryId}")]
        public async Task<IActionResult> GetByCountry(string countryId)
        {
            return Ok(await _service.GetByCountryAsync(countryId));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] LocationRequestDto dto)
        {
            var createdLocation =
                await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdLocation.LocationId },
                createdLocation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            decimal id,
            [FromBody] LocationRequestDto dto)
        {
            var updatedLocation =
                await _service.UpdateAsync(id, dto);

            return Ok(updatedLocation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            await _service.DeleteAsync(id);

            return Ok(new
            {
                message = "Location deleted successfully"
            });
        }
    }
}