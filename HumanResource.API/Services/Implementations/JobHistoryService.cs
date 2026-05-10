using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Interfaces;
namespace HumanResource.API.Services.Implementations
{
    public class JobHistoryService : IJobHistoryService
    {
        private readonly IJobHistoryRepository _jobHistoryRepository;
        private readonly IMapper _mapper;
        public JobHistoryService(IJobHistoryRepository jobHistoryRepository, IMapper mapper)
        {
            _jobHistoryRepository = jobHistoryRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<JobHistoryDto>> GetAllAsync()
        {
            var jobHistories = await _jobHistoryRepository.GetAllAsync();
            var jobHistoryDtos = _mapper.Map<IEnumerable<JobHistoryDto>>(jobHistories);
            return jobHistoryDtos;
        }
        public async Task<JobHistoryDto> GetByIdAsync(int employeeId)
        {
            var jobHistory = await _jobHistoryRepository.GetByIdAsync(employeeId);
            if (jobHistory == null)
            {
                throw new NotFoundException
                (
                    $"Job History for Employee Id {employeeId} not found"
                );
            }
            var jobHistoryDto = _mapper.Map<JobHistoryDto>(jobHistory);
            return jobHistoryDto;
        }
        public async Task<JobHistoryDto> AddAsync(JobHistoryDto jobHistoryDto)
        {
            var jobHistory = _mapper.Map<JobHistory>(jobHistoryDto);
            var addedJobHistory = await _jobHistoryRepository.AddAsync(jobHistory);
            var addedJobHistoryDto = _mapper.Map<JobHistoryDto>(addedJobHistory);
            return addedJobHistoryDto;
        }
        public async Task<bool> DeleteAsync(int employeeId)
        {
            var isDeleted = await _jobHistoryRepository.DeleteAsync(employeeId);
            if (!isDeleted)
            {
                throw new NotFoundException
                (
                    $"Job History for Employee Id {employeeId} not found"
                );
            }
            return true;
        }
        public async Task<IEnumerable<JobHistoryDto>> GetByDepartmentAsync(int departmentId)
        {
            var jobHistories = await _jobHistoryRepository.GetByDepartmentAsync(departmentId);
            return _mapper.Map<IEnumerable<JobHistoryDto>>(jobHistories);
        }
    }
}
