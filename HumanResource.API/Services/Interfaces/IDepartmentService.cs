using HumanResource.API.DTOs;

namespace HumanResource.API.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();

        Task<DepartmentDto> GetByIdAsync(decimal id);

        Task<IEnumerable<DepartmentDto>> GetByLocationAsync(decimal locationId);

        Task<DepartmentDto> AddAsync(DepartmentDto dto);

        Task<DepartmentDto> UpdateAsync(decimal id, DepartmentDto dto);

        Task<bool> DeleteAsync(decimal id);
    }
}