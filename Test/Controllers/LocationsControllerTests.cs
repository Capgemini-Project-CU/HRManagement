using FluentAssertions;
using HumanResource.API.Controllers;
using HumanResource.API.DTOs.LocationDto;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.TestData;

namespace Test.Controllers
{
    public class LocationsControllerTests
    {
        private readonly Mock<ILocationService> _serviceMock;
        private readonly LocationsController _controller;

        public LocationsControllerTests()
        {
            _serviceMock = new Mock<ILocationService>();

            _controller = new LocationsController(_serviceMock.Object);
        }

        // Positive Test Cases

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenLocationsExist()
        {
            var data = new List<LocationResponseDto>
            {
                LocationTestData.GetLocationResponseDto()
            };

            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(data);

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenLocationExists()
        {
            var dto = LocationTestData.GetLocationResponseDto();

            _serviceMock
                .Setup(x => x.GetByIdAsync(1000))
                .ReturnsAsync(dto);

            var result = await _controller.GetById(1000);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByCountry_ShouldReturnOk_WhenCountryExists()
        {
            var data = new List<LocationResponseDto>
            {
                LocationTestData.GetLocationResponseDto()
            };

            _serviceMock
                .Setup(x => x.GetByCountryAsync("IN"))
                .ReturnsAsync(data);

            var result = await _controller.GetByCountry("IN");

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenLocationCreated()
        {
            var requestDto = LocationTestData.GetLocationRequestDto();

            var responseDto = LocationTestData.GetLocationResponseDto();

            _serviceMock
                .Setup(x => x.CreateAsync(requestDto))
                .ReturnsAsync(responseDto);

            var result = await _controller.Create(requestDto);

            result.Should().BeOfType<CreatedAtActionResult>();
        }




        // Negative Test Cases
        [Fact]
        public async Task GetById_ShouldThrowException_WhenLocationNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByIdAsync(9999))
                .ThrowsAsync(
                    new Exception("Location not found"));

            Func<Task> act = async () => await _controller.GetById(9999);

            await act.Should().ThrowAsync<Exception>().WithMessage("Location not found");
        }

        [Fact]
        public async Task GetByCountry_ShouldThrowException_WhenCountryNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByCountryAsync("XX"))
                .ThrowsAsync(
                    new Exception("Country not found"));

            Func<Task> act = async () => await _controller.GetByCountry("XX");

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Country not found");
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenLocationNotFound()
        {
            var updateDto = LocationTestData.GetUpdateLocationDto();

            _serviceMock
                .Setup(x => x.UpdateAsync(9999, updateDto))
                .ThrowsAsync(
                    new Exception("Location not found"));

            Func<Task> act = async () => await _controller.Update(9999, updateDto);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Location not found");
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenLocationNotFound()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(9999))
                .ThrowsAsync(
                    new Exception("Location not found"));

            Func<Task> act = async () =>
                await _controller.Delete(9999);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Location not found");
        }
    }
}