using AutoMapper;
using FluentAssertions;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;
using Moq;
using Test.Helpers;

namespace Test.Services
{
    public class JobHistoryServiceTests
    {
        private readonly Mock<IJobHistoryRepository> _repositoryMock;

        private readonly IMapper _mapper;

        private readonly JobHistoryService _service;

        public JobHistoryServiceTests()
        {
            _repositoryMock =
                new Mock<IJobHistoryRepository>();

            _mapper =
                TestUtilities.GetMapper();

            _service =
                new JobHistoryService(
                    _repositoryMock.Object,
                    _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnJobHistories()
        {
            var data = new List<JobHistory>
            {
                new JobHistory
                {
                    EmployeeId = 100,
                    JobId = "IT_PROG",
                    DepartmentId = 60,
                    StartDate = new DateOnly(2024,1,1),
                    EndDate = new DateOnly(2025,1,1)
                }
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(data);

            var result =
                await _service.GetAllAsync();

            result.Should().NotBeNull();

            result.Count().Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnJobHistory_WhenExists()
        {
            var entity = new JobHistory
            {
                EmployeeId = 100,
                JobId = "IT_PROG",
                DepartmentId = 60,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2025, 1, 1)
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync(entity);

            var result =
                await _service.GetByIdAsync(100);

            result.Should().NotBeNull();

            result.EmployeeId.Should().Be(100);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenNotFound()
        {
            _repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((JobHistory)null);

            Func<Task> act = async () =>
                await _service.GetByIdAsync(999);

            await act.Should()
                .ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AddAsync_ShouldAddJobHistory()
        {
            var dto = new JobHistoryDto
            {
                EmployeeId = 100,
                JobId = "IT_PROG",
                DepartmentId = 60,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2025, 1, 1)
            };

            var entity = new JobHistory
            {
                EmployeeId = 100,
                JobId = "IT_PROG",
                DepartmentId = 60,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2025, 1, 1)
            };

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<JobHistory>()))
                .ReturnsAsync(entity);

            var result =
                await _service.AddAsync(dto);

            result.Should().NotBeNull();

            result.EmployeeId.Should().Be(100);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            _repositoryMock
                .Setup(x => x.DeleteAsync(100))
                .ReturnsAsync(true);

            var result =
                await _service.DeleteAsync(100);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenNotFound()
        {
            _repositoryMock
                .Setup(x => x.DeleteAsync(999))
                .ReturnsAsync(false);

            Func<Task> act = async () =>
                await _service.DeleteAsync(999);

            await act.Should()
                .ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetByDepartmentAsync_ShouldReturnRecords()
        {
            var data = new List<JobHistory>
            {
                new JobHistory
                {
                    EmployeeId = 100,
                    JobId = "IT_PROG",
                    DepartmentId = 60,
                    StartDate = new DateOnly(2024,1,1),
                    EndDate = new DateOnly(2025,1,1)
                }
            };

            _repositoryMock
                .Setup(x => x.GetByDepartmentAsync(60))
                .ReturnsAsync(data);

            var result =
                await _service.GetByDepartmentAsync(60);

            result.Should().NotBeNull();

            result.Count().Should().Be(1);
        }
    }
}