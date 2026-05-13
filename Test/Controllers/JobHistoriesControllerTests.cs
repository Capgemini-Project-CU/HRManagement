using FluentAssertions;
using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Controllers
{
    public class JobHistoryControllerTests
    {
        private readonly Mock<IJobHistoryService>
            _serviceMock;

        private readonly JobHistoryController
            _controller;

        public JobHistoryControllerTests()
        {
            _serviceMock =
                new Mock<IJobHistoryService>();

            _controller =
                new JobHistoryController(
                    _serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var data = new List<JobHistoryDto>
            {
                new JobHistoryDto
                {
                    EmployeeId = 100,
                    JobId = "IT_PROG",
                    DepartmentId = 60,
                    StartDate = new DateOnly(2024,1,1),
                    EndDate = new DateOnly(2025,1,1)
                }
            };

            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(data);

            var result =
                await _controller.GetAllJobHistory();

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenExists()
        {
            var dto = new JobHistoryDto
            {
                EmployeeId = 100,
                JobId = "IT_PROG",
                DepartmentId = 60,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2025, 1, 1)
            };

            _serviceMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync(dto);

            var result =
                await _controller.GetJobHistoryById(100);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WhenCreated()
        {
            var dto = new JobHistoryDto
            {
                EmployeeId = 100,
                JobId = "IT_PROG",
                DepartmentId = 60,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2025, 1, 1)
            };

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ReturnsAsync(dto);

            var result =
                await _controller.AddJobHistory(dto);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenDeleted()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(100))
                .ReturnsAsync(true);

            var result =
                await _controller.DeleteJobHistory(100);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByIdAsync(999))
                .ThrowsAsync(
                    new Exception("Job history not found"));

            Func<Task> act = async () =>
                await _controller.GetJobHistoryById(999);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Job history not found");
        }
    }
}