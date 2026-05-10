using HumanResource.API.DTOs;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;

namespace HumanResource.API.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _repository.GetAllAsync();

            return departments.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                ManagerId = d.ManagerId,
                LocationId = d.LocationId,

                ManagerName = d.Manager != null
                    ? d.Manager.FirstName + " " + d.Manager.LastName
                    : null,

                City = d.Location != null
                    ? d.Location.City
                    : null
            });
        }

        public async Task<DepartmentDto?> GetByIdAsync(decimal id)
        {
            var d = await _repository.GetByIdAsync(id);

            if (d == null)
                return null;

            return new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                ManagerId = d.ManagerId,
                LocationId = d.LocationId,

                ManagerName = d.Manager != null
                    ? d.Manager.FirstName + " " + d.Manager.LastName
                    : null,

                City = d.Location != null
                    ? d.Location.City
                    : null
            };
        }

        public async Task<IEnumerable<DepartmentDto>> GetByLocationAsync(decimal locationId)
        {
            var departments = await _repository.GetByLocationAsync(locationId);

            return departments.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                ManagerId = d.ManagerId,
                LocationId = d.LocationId,

                ManagerName = d.Manager != null
                    ? d.Manager.FirstName + " " + d.Manager.LastName
                    : null,

                City = d.Location != null
                    ? d.Location.City
                    : null
            });
        }

        public async Task<DepartmentDto> AddAsync(DepartmentDto dto)
        {
            var department = new Department
            {
                DepartmentName = dto.DepartmentName,
                ManagerId = dto.ManagerId,
                LocationId = dto.LocationId
            };

            var result = await _repository.AddAsync(department);

            dto.DepartmentId = result.DepartmentId;

            return dto;
        }

        public async Task<DepartmentDto?> UpdateAsync(decimal id, DepartmentDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                return null;

            existing.DepartmentName = dto.DepartmentName;
            existing.ManagerId = dto.ManagerId;
            existing.LocationId = dto.LocationId;

            await _repository.UpdateAsync(existing);

            return dto;
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}