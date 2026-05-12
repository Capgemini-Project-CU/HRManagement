using HumanResource.API.Models;

namespace HumanResource.API.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllAsync();

        Task<Country?> GetByIdAsync(string id);

        Task<IEnumerable<Country>> GetByRegionIdAsync(decimal regionId);

        Task<Country> AddAsync(Country country);

        Task<Country?> UpdateAsync(Country country);

        Task<bool> DeleteAsync(string id);
    }
}
