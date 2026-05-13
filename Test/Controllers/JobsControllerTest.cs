using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Controllers
{
    public class JobsControllerTests
    {
        private readonly Mock<IJobService> _mockService;

        private readonly JobsController _controller;

        public JobsControllerTests()
        {
            _mockService =
                new Mock<IJobService>();

            _controller =
                new JobsController(
                    _mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult()
        {
            // Arrange

            var jobs = new List<JobDto>
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

            _mockService.Setup(s =>
                s.GetAllAsync())
                .ReturnsAsync(jobs);

            // Act

            var result =
                await _controller.GetAll();

            // Assert

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedJobs =
                Assert.IsAssignableFrom<
                    IEnumerable<JobDto>>
                    (okResult.Value);

            Assert.Equal(2, returnedJobs.Count());
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOkResult()
        {
            // Arrange

            var job = new JobDto
            {
                JobId = "IT_PROG",
                JobTitle = "Programmer",
                MinSalary = 4000,
                MaxSalary = 10000
            };

            _mockService.Setup(s =>
                s.GetByIdAsync("IT_PROG"))
                .ReturnsAsync(job);

            // Act

            var result =
                await _controller.GetById(
                    "IT_PROG");

            // Assert

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedJob =
                Assert.IsType<JobDto>(
                    okResult.Value);

            Assert.Equal(
                "IT_PROG",
                returnedJob.JobId);
        }

        [Fact]
        public async Task Create_ValidJob_ReturnsOkResult()
        {
            // Arrange

            var dto = new JobDto
            {
                JobId = "DEV_JOB",
                JobTitle = "Developer",
                MinSalary = 5000,
                MaxSalary = 12000
            };

            _mockService.Setup(s =>
                s.CreateAsync(dto))
                .ReturnsAsync(dto);

            // Act

            var result =
                await _controller.Create(dto);

            // Assert

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedJob =
                Assert.IsType<JobDto>(
                    okResult.Value);

            Assert.Equal(
                dto.JobId,
                returnedJob.JobId);
        }

        [Fact]
        public async Task Update_ValidData_ReturnsOkResult()
        {
            // Arrange

            var dto = new JobDto
            {
                JobId = "IT_PROG",
                JobTitle = "Senior Developer",
                MinSalary = 7000,
                MaxSalary = 15000
            };

            _mockService.Setup(s =>
                s.UpdateAsync(
                    "IT_PROG",
                    dto))
                .ReturnsAsync(true);

            // Act

            var result =
                await _controller.Update(
                    "IT_PROG",
                    dto);

            // Assert

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                true,
                okResult.Value);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsOkResult()
        {
            // Arrange

            _mockService.Setup(s =>
                s.DeleteAsync("IT_PROG"))
                .ReturnsAsync(true);

            // Act

            var result =
                await _controller.Delete(
                    "IT_PROG");

            // Assert

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                "Job with Id IT_PROG deleted successfully",
                okResult.Value);
        }

        [Fact]
        public async Task GetBySalaryRange_ReturnsOkResult()
        {
            // Arrange

            var jobs = new List<JobDto>
            {
                new JobDto
                {
                    JobId = "IT_PROG",
                    JobTitle = "Programmer",
                    MinSalary = 4000,
                    MaxSalary = 10000
                }
            };

            _mockService.Setup(s =>
                s.GetBySalaryRangeAsync(
                    3000,
                    12000))
                .ReturnsAsync(jobs);

            // Act

            var result =
                await _controller
                    .GetBySalaryRange(
                        3000,
                        12000);

            // Assert

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedJobs =
                Assert.IsAssignableFrom<
                    IEnumerable<JobDto>>
                    (okResult.Value);

            Assert.Single(returnedJobs);
        }
    }
}