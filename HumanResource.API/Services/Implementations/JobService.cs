using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;

namespace HumanResource.API.Services.Implementations
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _repository;

        private readonly IMapper _mapper;

        public JobService(IJobRepository repository, IMapper mapper)
        {
            _repository = repository;

            _mapper = mapper;
        }

        public async Task<IEnumerable<JobDto>> GetAllAsync()
        {
            var jobs = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<JobDto>>(jobs);
        }
        public async Task<IEnumerable<JobDto>> GetBySalaryRangeAsync(decimal min, decimal max)
        {
            var jobs = await _repository.GetBySalaryRangeAsync(min, max);

            return _mapper.Map<IEnumerable<JobDto>>(jobs);
        }

        public async Task<JobDto?> GetByIdAsync(string id)
        {
            var job = await _repository.GetByIdAsync(id);

            if (job == null)
                return null;

            return _mapper.Map<JobDto>(job);
        }

        public async Task<JobDto> CreateAsync(JobDto dto)
        {
            var job = _mapper.Map<Job>(dto);

            await _repository.AddAsync(job);

            return _mapper.Map<JobDto>(job);
        }

        public async Task<bool> UpdateAsync(string id, JobDto dto)
        {
            var existingJob = await _repository.GetByIdAsync(id);

            if (existingJob == null)
                return false;

            _mapper.Map(dto, existingJob);

            await _repository.UpdateAsync(existingJob);

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var job = await _repository.GetByIdAsync(id);

            if (job == null)
                return false;

            await _repository.DeleteAsync(job);

            return true;
        }
    }
}