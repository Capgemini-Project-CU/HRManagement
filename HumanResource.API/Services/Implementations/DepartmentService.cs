using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;

namespace HumanResource.API.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        private readonly IMapper _mapper;

        public DepartmentService(
            IDepartmentRepository repository,
            IMapper mapper)
        {
            _repository = repository;

            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto?> GetByIdAsync(decimal id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                return null;

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<IEnumerable<DepartmentDto>> GetByLocationAsync(decimal locationId)
        {
            var departments = await _repository.GetByLocationAsync(locationId);

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto> AddAsync(DepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);

            var result = await _repository.AddAsync(department);

            return _mapper.Map<DepartmentDto>(result);
        }

        public async Task<DepartmentDto?> UpdateAsync(decimal id, DepartmentDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                return null;

            _mapper.Map(dto, existing);

            var updatedDepartment = await _repository.UpdateAsync(existing);

            return _mapper.Map<DepartmentDto>(updatedDepartment);
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}