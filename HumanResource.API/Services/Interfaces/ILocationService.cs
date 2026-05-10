using HumanResource.API.DTOs;
using HumanResource.API.DTOs.LocationDto;

namespace HumanResource.API.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationResponseDto>> GetAllAsync();

        Task<LocationResponseDto?> GetByIdAsync(decimal id);

        Task<IEnumerable<LocationResponseDto>> GetByCountryAsync(string countryId);

        Task<LocationResponseDto> CreateAsync(LocationRequestDto dto);

        Task<LocationResponseDto?> UpdateAsync(decimal id, LocationRequestDto dto);

        Task<bool> DeleteAsync(decimal id);
    }
}
