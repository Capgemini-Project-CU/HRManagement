using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;
using Test.TestData;
using Moq;

namespace HumanResource.API.Tests.Services
{
    public class JobServiceTests
    {
        private readonly Mock<IJobRepository> _mockRepo;

        private readonly Mock<IMapper> _mockMapper;

        private readonly JobService _service;

        public JobServiceTests()
        {
            _mockRepo =
                new Mock<IJobRepository>();

            _mockMapper =
                new Mock<IMapper>();

            _service = new JobService(
                _mockRepo.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllJobs()
        {
            // Arrange

            var jobs = JobTestData.GetJobs();

            var jobDtos = new List<JobDto>
            {
                new JobDto
                {
                    JobId = "IT_PROG",
                    JobTitle = "Programmer",
                    MinSalary = 4000,
                    MaxSalary = 10000
                },

                new JobDto
                {
                    JobId = "HR_REP",
                    JobTitle = "HR Representative",
                    MinSalary = 3000,
                    MaxSalary = 8000
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(jobs);

            _mockMapper.Setup(m =>
                m.Map<IEnumerable<JobDto>>(jobs))
                .Returns(jobDtos);

            // Act

            var result = await _service.GetAllAsync();

            // Assert

            Assert.NotNull(result);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsJob()
        {
            // Arrange

            var job = JobTestData.GetJob();

            var jobDto = new JobDto
            {
                JobId = "IT_PROG",
                JobTitle = "Programmer",
                MinSalary = 4000,
                MaxSalary = 10000
            };

            _mockRepo.Setup(r =>
                r.GetByIdAsync("IT_PROG"))
                .ReturnsAsync(job);

            _mockMapper.Setup(m =>
                m.Map<JobDto>(job))
                .Returns(jobDto);

            // Act

            var result =
                await _service.GetByIdAsync("IT_PROG");

            // Assert

            Assert.NotNull(result);

            Assert.Equal(
                "IT_PROG",
                result.JobId);

            Assert.Equal(
                "Programmer",
                result.JobTitle);
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange

            _mockRepo.Setup(r =>
                r.GetByIdAsync("INVALID"))
                .ReturnsAsync((Job?)null);

            // Act & Assert

            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetByIdAsync("INVALID"));
        }

        [Fact]
        public async Task CreateAsync_ValidJob_ReturnsCreatedJob()
        {
            // Arrange

            var dto = JobTestData.GetJobDto();

            var job = new Job
            {
                JobId = dto.JobId,
                JobTitle = dto.JobTitle,
                MinSalary = dto.MinSalary,
                MaxSalary = dto.MaxSalary
            };

            _mockRepo.Setup(r =>
                r.GetByIdAsync(dto.JobId))
                .ReturnsAsync((Job?)null);

            _mockMapper.Setup(m =>
                m.Map<Job>(dto))
                .Returns(job);

            _mockRepo.Setup(r =>
                r.AddAsync(It.IsAny<Job>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m =>
                m.Map<JobDto>(job))
                .Returns(dto);

            // Act

            var result =
                await _service.CreateAsync(dto);

            // Assert

            Assert.NotNull(result);

            Assert.Equal(
                dto.JobId,
                result.JobId);

            _mockRepo.Verify(
                r => r.AddAsync(It.IsAny<Job>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateJob_ThrowsBadRequestException()
        {
            // Arrange

            var dto = JobTestData.GetJobDto();

            var existingJob = JobTestData.GetJob();

            _mockRepo.Setup(r =>
                r.GetByIdAsync(dto.JobId))
                .ReturnsAsync(existingJob);

            // Act & Assert

            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_ValidData_ReturnsTrue()
        {
            // Arrange

            var existingJob = JobTestData.GetJob();

            var updatedDto =
                JobTestData.GetUpdatedJobDto();

            _mockRepo.Setup(r =>
                r.GetByIdAsync("IT_PROG"))
                .ReturnsAsync(existingJob);

            _mockRepo.Setup(r =>
                r.UpdateAsync(existingJob))
                .Returns(Task.CompletedTask);

            // Act

            var result =
                await _service.UpdateAsync(
                    "IT_PROG",
                    updatedDto);

            // Assert

            Assert.True(result);

            _mockRepo.Verify(
                r => r.UpdateAsync(existingJob),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange

            var dto =
                JobTestData.GetUpdatedJobDto();

            _mockRepo.Setup(r =>
                r.GetByIdAsync("INVALID"))
                .ReturnsAsync((Job?)null);

            // Act & Assert

            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.UpdateAsync(
                    "INVALID",
                    dto));
        }

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsTrue()
        {
            // Arrange

            var job = JobTestData.GetJob();

            _mockRepo.Setup(r =>
                r.GetByIdAsync("IT_PROG"))
                .ReturnsAsync(job);

            _mockRepo.Setup(r =>
                r.DeleteAsync(job))
                .Returns(Task.CompletedTask);

            // Act

            var result =
                await _service.DeleteAsync("IT_PROG");

            // Assert

            Assert.True(result);

            _mockRepo.Verify(
                r => r.DeleteAsync(job),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange

            _mockRepo.Setup(r =>
                r.GetByIdAsync("INVALID"))
                .ReturnsAsync((Job?)null);

            // Act & Assert

            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.DeleteAsync("INVALID"));
        }
    }
}