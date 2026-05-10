using AutoMapper;
using HumanResource.API.DTOs.LocationDto;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;

namespace HumanResource.API.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repository;
        private readonly IMapper _mapper;

        public LocationService(
            ILocationRepository repository,
            IMapper mapper)
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
                throw new NotFoundException($"Location with Id {id} not found");

            return _mapper.Map<LocationResponseDto>(location);
        }

        public async Task<IEnumerable<LocationResponseDto>> GetByCountryAsync(string countryId)
        {
            var locations = await _repository.GetByCountryAsync(countryId);

            return _mapper.Map<IEnumerable<LocationResponseDto>>(locations);
        }

        public async Task<LocationResponseDto> CreateAsync(LocationRequestDto dto)
        {
            var countryExists =
                await _repository.CountryExistsAsync(dto.CountryId);

            if (!countryExists)
                throw new BadRequestException("Invalid Country Id");

            var location = _mapper.Map<Location>(dto);

            var created = await _repository.AddAsync(location);

            var createdLocation =
                await _repository.GetByIdAsync(created.LocationId);

            return _mapper.Map<LocationResponseDto>(createdLocation);
        }

        public async Task<LocationResponseDto?> UpdateAsync(
            decimal id,
            LocationRequestDto dto)
        {
            var existingLocation =
                await _repository.GetByIdAsync(id);

            if (existingLocation == null)
                throw new NotFoundException(
                    $"Location with Id {id} not found");

            var countryExists =
                await _repository.CountryExistsAsync(dto.CountryId);

            if (!countryExists)
                throw new BadRequestException("Invalid Country Id");

            var location = _mapper.Map<Location>(dto);

            location.LocationId = id;

            var updated =
                await _repository.UpdateAsync(location);

            return _mapper.Map<LocationResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException(
                    $"Location with Id {id} not found");

            return true;
        }
    }
}