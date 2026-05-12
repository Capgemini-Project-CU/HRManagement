using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;

namespace HumanResource.API.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        private readonly IMapper _mapper;

        public RoleService(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;

            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetByIdAsync(int id)
        {
            var role = await _repository.GetByIdAsync(id);

            if (role == null)
                throw new NotFoundException("Role not found");

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> CreateAsync(RoleDto dto)
        {
            var role = _mapper.Map<Role>(dto);

            await _repository.AddAsync(role);

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> UpdateAsync(int id, RoleDto dto)
        {
            var existingRole = await _repository.GetByIdAsync(id);

            if (existingRole == null)
                throw new NotFoundException("Role not found");

            _mapper.Map(dto, existingRole);

            await _repository.UpdateAsync(existingRole);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _repository.GetByIdAsync(id);

            if (role == null)
                throw new NotFoundException("Role not found");

            await _repository.DeleteAsync(role);

            return true;
        }
    }
}