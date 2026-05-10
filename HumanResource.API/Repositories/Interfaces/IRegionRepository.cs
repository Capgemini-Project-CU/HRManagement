using HumanResource.API.Models;

namespace HumanResource.API.Repositories.Interfaces
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllAsync();

        Task<Region?> GetByIdAsync(decimal id);

        Task<Region> AddAsync(Region region);

        Task<Region?> UpdateAsync(Region region);

        Task<bool> DeleteAsync(decimal id);
    }
}
