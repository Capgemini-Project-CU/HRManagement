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
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationService service, ILogger<LocationsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/locations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _service.GetAllAsync();
            return Ok(locations);
        }

        // GET: api/locations/1000
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var location = await _service.GetByIdAsync(id);

            if (location == null)
                return NotFound(new { message = "Location not found" });

            return Ok(location);
        }

        // GET: api/locations/country/IN
        [HttpGet("country/{countryId}")]
        public async Task<IActionResult> GetByCountry(string countryId)
        {
            var locations = await _service.GetByCountryAsync(countryId);
            return Ok(locations);
        }

        // POST: api/locations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LocationRequestDto dto)
        {
            var createdLocation = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdLocation.LocationId }, // ⚠️ requires LocationId in DTO
                createdLocation
            );
        }

        // PUT: api/locations/1000
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(decimal id, [FromBody] LocationRequestDto dto)
        {
            var updatedLocation = await _service.UpdateAsync(id, dto);

            if (updatedLocation == null)
                return NotFound(new { message = "Location not found" });

            return Ok(updatedLocation);
        }

        // DELETE: api/locations/1000
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Location not found" });

            return Ok(new { message = "Location deleted successfully" });
        }
    }
}