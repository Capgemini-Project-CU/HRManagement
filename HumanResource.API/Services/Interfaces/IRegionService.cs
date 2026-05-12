using HumanResource.API.DTOs;

namespace HumanResource.API.Services.Interfaces
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionDto>> GetAllAsync();
        Task<RegionDto> GetByIdAsync(decimal id);
        Task<RegionDto> AddAsync(RegionDto dto);
        Task<RegionDto> UpdateAsync(decimal id, RegionDto dto);
        Task<bool> DeleteAsync(decimal id);
    }
}
