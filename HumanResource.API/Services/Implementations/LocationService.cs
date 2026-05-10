using HumanResource.API.DTOs.LocationDto;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;
using AutoMapper;

namespace HumanResource.API.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationResponseDto>> GetAllAsync()
        {
            var locations = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<LocationResponseDto>>(locations);
        }

        public async Task<LocationResponseDto?> GetByIdAsync(decimal id)
        {
            var location = await _repository.GetByIdAsync(id);

            if (location == null)
                return null;

            return _mapper.Map<LocationResponseDto>(location);
        }

        public async Task<IEnumerable<LocationResponseDto>> GetByCountryAsync(string countryId)
        {
            var locations = await _repository.GetByCountryAsync(countryId);

            return _mapper.Map<IEnumerable<LocationResponseDto>>(locations);
        }

        public async Task<LocationResponseDto> CreateAsync(LocationRequestDto dto)
        {
            // Foreign Key Validation
            var countryExists = await _repository
                .CountryExistsAsync(dto.CountryId);

            if (!countryExists)
            {
                throw new Exception("Invalid Country Id");
            }

            var location = _mapper.Map<Location>(dto);

            var created = await _repository.AddAsync(location);

            var createdLocation = await _repository
                .GetByIdAsync(created.LocationId);

            return _mapper.Map<LocationResponseDto>(createdLocation);

        }

        public async Task<LocationResponseDto?> UpdateAsync(decimal id, LocationRequestDto dto)
        {
            // Foreign Key Validation
            var countryExists = await _repository
                .CountryExistsAsync(dto.CountryId);

            if (!countryExists)
            {
                throw new Exception("Invalid Country Id");
            }

            var location = _mapper.Map<Location>(dto);

            location.LocationId = location.LocationId;

            var updated = await _repository.UpdateAsync(location);

            if (updated == null)
                return null;

            var updatedLocation = await _repository
                .GetByIdAsync(updated.LocationId);

            return _mapper.Map<LocationResponseDto>(updatedLocation);

        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            return await _repository.DeleteAsync(id);
        }

        
    }
}
