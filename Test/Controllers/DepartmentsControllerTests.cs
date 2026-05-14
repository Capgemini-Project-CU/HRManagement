using FluentAssertions;
using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.TestData;

namespace Test.Controllers
{
    public class DepartmentsControllerTests
    {
        private readonly Mock<IDepartmentService> _serviceMock;

        private readonly DepartmentsController _controller;

        public DepartmentsControllerTests()
        {
            _serviceMock = new Mock<IDepartmentService>();

            _controller = new DepartmentsController(
                _serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var data = new List<DepartmentDto>
            {
                DepartmentTestData.GetDepartmentDto()
            };

            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(data);

            var result = await _controller.GetAll();

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenDepartmentExists()
        {
            var dto = DepartmentTestData.GetDepartmentDto();

            _serviceMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(dto);

            var result = await _controller.GetById(10);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WhenDepartmentCreated()
        {
            var dto = DepartmentTestData.GetDepartmentDto();

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ReturnsAsync(dto);

            var result = await _controller.Create(dto);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenDepartmentUpdated()
        {
            var dto = DepartmentTestData.GetDepartmentDto();

            _serviceMock
                .Setup(x => x.UpdateAsync(10, dto))
                .ReturnsAsync(dto);

            var result = await _controller.Update(10, dto);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenDepartmentNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync((DepartmentDto?)null);

            var result = await _controller.GetById(100);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenDepartmentNotFound()
        {
            var dto = DepartmentTestData.GetDepartmentDto();

            _serviceMock
                .Setup(x => x.UpdateAsync(100, dto))
                .ReturnsAsync((DepartmentDto?)null);

            var result = await _controller.Update(100, dto);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenDepartmentNotFound()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(100))
                .ThrowsAsync(new Exception("Department not found"));

            Func<Task> act = async () =>
                await _controller.Delete(100);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Department not found");
        }

        [Fact]
        public async Task GetByLocation_ShouldThrowException_WhenLocationNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByLocationAsync(999))
                .ThrowsAsync(new Exception("No departments found"));

            Func<Task> act = async () =>
                await _controller.GetByLocation(999);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("No departments found");
        }
    }
}