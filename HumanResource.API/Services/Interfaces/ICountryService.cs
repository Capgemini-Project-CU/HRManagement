using HumanResource.API.DTOs;

namespace HumanResource.API.Services.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDto>> GetAllAsync();
        Task<CountryDto> GetByIdAsync(string id);
        Task<IEnumerable<CountryDto>> GetByRegionIdAsync(decimal regionId);
        Task<CountryDto> AddAsync(CountryDto dto);
        Task<CountryDto> UpdateAsync(string id, CountryDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
