using FluentAssertions;
using HumanResource.API.Controllers;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.TestData;

namespace Test.Controllers
{
    public class LocationsControllerTests
    {
        private readonly Mock<ILocationService>  _serviceMock;
        private readonly LocationsController _controller;
        public LocationsControllerTests()
        {
            _serviceMock = new Mock<ILocationService>();
            _controller = new LocationsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenLocationExists()
        {
            var dto = LocationTestData.GetLocationResponseDto();
            _serviceMock.Setup(x => x.GetByIdAsync(1000)).ReturnsAsync(dto);
            var result = await _controller.GetById(1000);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenLocationCreated()
        {
            var requestDto = LocationTestData.GetLocationRequestDto();
            var responseDto = LocationTestData.GetLocationResponseDto();
            _serviceMock.Setup(x => x.CreateAsync(requestDto)).ReturnsAsync(responseDto);
            var result = await _controller.Create(requestDto);
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenLocationNotFound()
        {
            _serviceMock.Setup(x => x.GetByIdAsync(9999)).ThrowsAsync(new Exception("Location not found"));
            Func<Task> act = async () => await _controller.GetById(9999);
            await act.Should().ThrowAsync<Exception>().WithMessage("Location not found");
        }


        [Fact]
        public async Task Delete_ShouldThrowException_WhenLocationNotFound()
        {
            _serviceMock.Setup(x => x.DeleteAsync(9999)).ThrowsAsync(new Exception("Location not found"));
            Func<Task> act = async () => await _controller.Delete(9999);
            await act.Should().ThrowAsync<Exception>().WithMessage("Location not found");
        }

    }
}
