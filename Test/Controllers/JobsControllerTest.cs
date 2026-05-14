using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.TestData;
using static System.Net.Mime.MediaTypeNames;

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
        public async Task GetAll_ReturnsOkResult_WithJobs()
        {
            // Arrange

            var jobs = JobTestData.GetJobs()
                .Select(j => new JobDto
                {
                    JobId = j.JobId,
                    JobTitle = j.JobTitle,
                    MinSalary = j.MinSalary,
                    MaxSalary = j.MaxSalary
                })
                .ToList();

            _mockService.Setup(s =>
                s.GetAllAsync())
                .ReturnsAsync(jobs);


            var result =
                await _controller.GetAll();


            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedJobs =
                Assert.IsAssignableFrom<
                    IEnumerable<JobDto>>(
                        okResult.Value);

            Assert.Equal(2, returnedJobs.Count());
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsCorrectJob()
        {

            var dto = JobTestData.GetJobDto();

            _mockService.Setup(s =>
                s.GetByIdAsync("DEV_JOB"))
                .ReturnsAsync(dto);


            var result =
                await _controller.GetById("DEV_JOB");


            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedJob =
                Assert.IsType<JobDto>(
                    okResult.Value);

            Assert.Equal(
                "DEV_JOB",
                returnedJob.JobId);
        }

        [Fact]
        public async Task Create_ValidJob_ReturnsOkResult()
        {

            var dto = JobTestData.GetJobDto();

            _mockService.Setup(s =>
                s.CreateAsync(dto))
                .ReturnsAsync(dto);


            var result =
                await _controller.Create(dto);


            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedJob =
                Assert.IsType<JobDto>(
                    okResult.Value);

            Assert.Equal(
                dto.JobTitle,
                returnedJob.JobTitle);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsSuccessMessage()
        {

            _mockService.Setup(s =>
                s.DeleteAsync("DEV_JOB"))
                .ReturnsAsync(true);


            var result =
                await _controller.Delete("DEV_JOB");


            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                "Job with Id DEV_JOB deleted successfully",
                okResult.Value);
        }

        [Fact]
        public async Task GetById_InvalidId_ThrowsNotFoundException()
        {

            _mockService.Setup(s =>
                s.GetByIdAsync("INVALID"))
                .ThrowsAsync(
                    new NotFoundException(
                        "Job not found"));


            await Assert.ThrowsAsync<
                NotFoundException>(() =>
                _controller.GetById("INVALID"));
        }

        [Fact]
        public async Task Create_DuplicateJob_ThrowsBadRequestException()
        {

            var dto = JobTestData.GetJobDto();

            _mockService.Setup(s =>
                s.CreateAsync(dto))
                .ThrowsAsync(
                    new BadRequestException(
                        "Job already exists"));


            await Assert.ThrowsAsync<
                BadRequestException>(() =>
                _controller.Create(dto));
        }

        [Fact]
        public async Task Update_InvalidId_ThrowsNotFoundException()
        {

            var dto =
                JobTestData.GetUpdatedJobDto();

            _mockService.Setup(s =>
                s.UpdateAsync(
                    "INVALID",
                    dto))
                .ThrowsAsync(
                    new NotFoundException(
                        "Job not found"));

            await Assert.ThrowsAsync<
                NotFoundException>(() =>
                _controller.Update(
                    "INVALID",
                    dto));
        }

        [Fact]
        public async Task Delete_InvalidId_ThrowsNotFoundException()
        {

            _mockService.Setup(s =>
                s.DeleteAsync("INVALID"))
                .ThrowsAsync(
                    new NotFoundException(
                        "Job not found"));

            await Assert.ThrowsAsync<
                NotFoundException>(() =>
                _controller.Delete("INVALID"));
        }
    }
}