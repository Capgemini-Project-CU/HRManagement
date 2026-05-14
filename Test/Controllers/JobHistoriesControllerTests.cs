using FluentAssertions;
using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.TestData;

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

        // ---------------- POSITIVE TEST CASES ----------------

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var data = new List<JobHistoryDto>
            {
                JobHistoryTestData.GetJobHistoryDto()
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
            var dto =
                JobHistoryTestData.GetJobHistoryDto();

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
            var dto =
                JobHistoryTestData.GetJobHistoryDto();

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

        // ---------------- NEGATIVE TEST CASES ----------------

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

        [Fact]
        public async Task Add_ShouldThrowException_WhenInvalid()
        {
            var dto =
                JobHistoryTestData.GetJobHistoryDto();

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ThrowsAsync(
                    new Exception("Invalid job history"));

            Func<Task> act = async () =>
                await _controller.AddJobHistory(dto);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Invalid job history");
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenNotFound()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(999))
                .ThrowsAsync(
                    new Exception("Job history not found"));

            Func<Task> act = async () =>
                await _controller.DeleteJobHistory(999);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Job history not found");
        }

        [Fact]
        public async Task GetAll_ShouldThrowException_WhenFailed()
        {
            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ThrowsAsync(
                    new Exception("Database error"));

            Func<Task> act = async () =>
                await _controller.GetAllJobHistory();

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Database error");
        }
    }
}