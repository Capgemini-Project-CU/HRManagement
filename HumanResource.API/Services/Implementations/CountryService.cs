using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;

namespace HumanResource.API.Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repository;
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        public CountryService(
            ICountryRepository repository,
            IRegionRepository regionRepository,
            IMapper mapper)
        {
            _repository = repository;
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CountryDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<CountryDto>>(data);
        }

        public async Task<CountryDto> GetByIdAsync(string id)
        {
            var data = await _repository.GetByIdAsync(id);

            if (data == null)
            {
                throw new NotFoundException(
                    $"Country with ID {id} not found.");
            }

            return _mapper.Map<CountryDto>(data);
        }

        public async Task<IEnumerable<CountryDto>>
            GetByRegionIdAsync(decimal regionId)
        {
            var data =
                await _repository.GetByRegionIdAsync(regionId);

            return _mapper.Map<IEnumerable<CountryDto>>(data);
        }

        public async Task<CountryDto> AddAsync(CountryDto dto)
        {
            // Duplicate Country Validation
            var existingCountry =
                await _repository.GetByIdAsync(dto.CountryId!);

            if (existingCountry != null)
            {
                throw new ConflictException(
                    $"Country with ID {dto.CountryId} already exists.");
            }

            // Region Exists Validation
            var existingRegion =
                await _regionRepository.GetByIdAsync(dto.RegionId);

            if (existingRegion == null)
            {
                throw new BadRequestException(
                    $"Region with ID {dto.RegionId} does not exist.");
            }

            var entity = _mapper.Map<Country>(dto);

            await _repository.AddAsync(entity);

            var insertedCountry =
                await _repository.GetByIdAsync(entity.CountryId);

            return _mapper.Map<CountryDto>(insertedCountry);
        }

        public async Task<CountryDto> UpdateAsync(
            string id,
            CountryDto dto)
        {
            if (!string.IsNullOrEmpty(dto.CountryId)
                && id != dto.CountryId)
            {
                throw new BadRequestException(
                    "Country ID mismatch.");
            }

            dto.CountryId = id;

            // Region Exists Validation
            var existingRegion =
                await _regionRepository.GetByIdAsync(dto.RegionId);

            if (existingRegion == null)
            {
                throw new BadRequestException(
                    $"Region with ID {dto.RegionId} does not exist.");
            }

            var entity = _mapper.Map<Country>(dto);

            var result = await _repository.UpdateAsync(entity);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Country with ID {id} not found.");
            }

            var updatedCountry =
                await _repository.GetByIdAsync(id);

            return _mapper.Map<CountryDto>(updatedCountry);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existingCountry =
                await _repository.GetByIdAsync(id);

            if (existingCountry == null)
            {
                throw new NotFoundException(
                    $"Country with ID {id} not found.");
            }

            return await _repository.DeleteAsync(id);
        }
    }
}