using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;

namespace HumanResource.API.Services.Implementations
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _repository;
        private readonly IMapper _mapper;

        public RegionService(
            IRegionRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RegionDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<RegionDto>>(data);
        }

        public async Task<RegionDto> GetByIdAsync(decimal id)
        {
            var data = await _repository.GetByIdAsync(id);

            if (data == null)
            {
                throw new NotFoundException(
                    $"Region with ID {id} not found.");
            }

            return _mapper.Map<RegionDto>(data);
        }

        public async Task<RegionDto> AddAsync(RegionDto dto)
        {
            var existingRegion =
                await _repository.GetByIdAsync(dto.RegionId);

            if (existingRegion != null)
            {
                throw new ConflictException(
                    $"Region with ID {dto.RegionId} already exists.");
            }

            var entity = _mapper.Map<Region>(dto);

            var result = await _repository.AddAsync(entity);

            return _mapper.Map<RegionDto>(result);
        }

        public async Task<RegionDto> UpdateAsync(
            decimal id,
            RegionDto dto)
        {
            if (dto.RegionId != 0 && id != dto.RegionId)
            {
                throw new BadRequestException(
                    "Region ID mismatch.");
            }

            dto.RegionId = id;

            var entity = _mapper.Map<Region>(dto);

            var result = await _repository.UpdateAsync(entity);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Region with ID {id} not found.");
            }

            return _mapper.Map<RegionDto>(result);
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            var existingRegion =
                await _repository.GetByIdAsync(id);

            if (existingRegion == null)
            {
                throw new NotFoundException(
                    $"Region with ID {id} not found.");
            }

            if (existingRegion.Countries.Any())
            {
                throw new ConflictException(
                    "Cannot delete region because countries exist under it.");
            }

            return await _repository.DeleteAsync(id);
        }
    }
}