using FluentAssertions;
using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.TestData;

namespace Test.Controllers
{
    public class CountriesControllerTests
    {
        private readonly Mock<ICountryService>
            _serviceMock;

        private readonly CountriesController
            _controller;

        public CountriesControllerTests()
        {
            _serviceMock =
                new Mock<ICountryService>();

            _controller =
                new CountriesController(
                    _serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var data = new List<CountryDto>
            {
                CountryTestData.GetCountryDto()
            };

            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(data);

            var result =
                await _controller.GetAll();

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCountryExists()
        {
            var dto =
                CountryTestData.GetCountryDto();

            _serviceMock
                .Setup(x => x.GetByIdAsync("IN"))
                .ReturnsAsync(dto);

            var result =
                await _controller.GetById("IN");

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WhenCountryCreated()
        {
            var dto =
                CountryTestData.GetCountryDto();

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ReturnsAsync(dto);

            var result =
                await _controller.Create(dto);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenDeleted()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync("IN"))
                .ReturnsAsync(true);

            var result =
                await _controller.Delete("IN");

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenCountryNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByIdAsync("XX"))
                .ThrowsAsync(
                    new Exception("Country not found"));

            Func<Task> act = async () =>
                await _controller.GetById("XX");

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Country not found");
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenCountryAlreadyExists()
        {
            var dto =
                CountryTestData.GetCountryDto();

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ThrowsAsync(
                    new Exception("Country already exists"));

            Func<Task> act = async () =>
                await _controller.Create(dto);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Country already exists");
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenCountryNotFound()
        {
            var dto =
                CountryTestData.GetCountryDto();

            _serviceMock
                .Setup(x => x.UpdateAsync("XX", dto))
                .ThrowsAsync(
                    new Exception("Country not found"));

            Func<Task> act = async () =>
                await _controller.Update("XX", dto);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Country not found");
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenCountryNotFound()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync("XX"))
                .ThrowsAsync(
                    new Exception("Country not found"));

            Func<Task> act = async () =>
                await _controller.Delete("XX");

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Country not found");
        }
    }
}
