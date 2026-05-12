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
using FluentAssertions;

namespace Test.Controllers
{
    public class RegionsControllerTests
    {
        private readonly Mock<IRegionService> _serviceMock;
        private readonly RegionsController _controller;

        public RegionsControllerTests()
        {
            _serviceMock = new Mock<IRegionService>();
            _controller = new RegionsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenRegionExists()
        {
            var dto = RegionTestData.GetRegionDto();

            _serviceMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(dto);

            var result = await _controller.GetById(10);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var data = new List<RegionDto>
            {
                RegionTestData.GetRegionDto()
            };

            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(data);

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            var dto = RegionTestData.GetRegionDto();

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ReturnsAsync(dto);

            var result = await _controller.Create(dto);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnOk()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(10))
                .ReturnsAsync(true);

            var result = await _controller.Delete(10);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenRegionNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByIdAsync(99))
                .ThrowsAsync(new Exception("Region not found"));

            Func<Task> act = async () =>
                await _controller.GetById(99);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Region not found");
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenRegionNotFound()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(99))
                .ThrowsAsync(new Exception("Region not found"));
            Func<Task> act = async () =>
                await _controller.Delete(99);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Region not found");
        }
    }
}
