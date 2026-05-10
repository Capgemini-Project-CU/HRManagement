using HumanResource.API.Models;

namespace HumanResource.API.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllAsync();

        Task<Location?> GetByIdAsync(decimal id);

        Task<IEnumerable<Location>> GetByCountryAsync(string countryId);

        Task<Location> AddAsync(Location location);

        Task<Location?> UpdateAsync(Location location);

        Task<bool> DeleteAsync(decimal id);

        Task<bool> CountryExistsAsync(string countryId);
    }
}
