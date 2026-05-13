using AutoMapper;
using FluentAssertions;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Helpers;
using Test.TestData;

namespace Test.Services
{
    public class RegionServiceTests
    {
        private readonly Mock<IRegionRepository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly RegionService _service;

        public RegionServiceTests()
        {
            _repositoryMock = new Mock<IRegionRepository>();

            _mapper = TestUtilities.GetMapper();

            _service = new RegionService(
                _repositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRegion_WhenExists()
        {
            var region = RegionTestData.GetRegionEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(region);

            var result = await _service.GetByIdAsync(10);

            result.Should().NotBeNull();

            result.RegionId.Should().Be(10);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenNotFound()
        {
            _repositoryMock
                .Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((Region)null);

            Func<Task> act = async () =>
                await _service.GetByIdAsync(99);

            await act.Should()
                .ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AddAsync_ShouldCreateRegion_WhenValid()
        {
            var dto = RegionTestData.GetRegionDto();

            var entity = RegionTestData.GetRegionEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(dto.RegionId))
                .ReturnsAsync((Region)null);

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Region>()))
                .ReturnsAsync(entity);

            var result = await _service.AddAsync(dto);

            result.Should().NotBeNull();

            result.RegionName.Should().Be("Europe");
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenDuplicateExists()
        {
            var dto = RegionTestData.GetRegionDto();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(dto.RegionId))
                .ReturnsAsync(RegionTestData.GetRegionEntity());

            Func<Task> act = async () =>
                await _service.AddAsync(dto);

            await act.Should()
                .ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenIdMismatch()
        {
            var dto = RegionTestData.GetRegionDto();

            dto.RegionId = 20;

            Func<Task> act = async () =>
                await _service.UpdateAsync(10, dto);

            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            var region = RegionTestData.GetRegionEntity();

            region.Countries = new List<Country>();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(region);

            _repositoryMock
                .Setup(x => x.DeleteAsync(10))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(10);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenCountriesExist()
        {
            var region = RegionTestData.GetRegionEntity();

            region.Countries = new List<Country>
            {
                new Country()
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(region);

            Func<Task> act = async () =>
                await _service.DeleteAsync(10);

            await act.Should()
                .ThrowAsync<ConflictException>();
        }
    }
}
