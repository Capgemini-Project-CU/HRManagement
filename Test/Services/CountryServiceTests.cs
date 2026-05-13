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
    public class CountryServiceTests
    {
        private readonly Mock<ICountryRepository>
            _repositoryMock;

        private readonly Mock<IRegionRepository>
            _regionRepositoryMock;

        private readonly IMapper _mapper;

        private readonly CountryService _service;

        public CountryServiceTests()
        {
            _repositoryMock =
                new Mock<ICountryRepository>();

            _regionRepositoryMock =
                new Mock<IRegionRepository>();

            _mapper = TestUtilities.GetMapper();

            _service = new CountryService(
                _repositoryMock.Object,
                _regionRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCountry_WhenExists()
        {
            var country =
                CountryTestData.GetCountryEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync("IN"))
                .ReturnsAsync(country);

            var result =
                await _service.GetByIdAsync("IN");

            result.Should().NotBeNull();

            result.CountryId.Should().Be("IN");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenNotFound()
        {
            _repositoryMock
                .Setup(x => x.GetByIdAsync("XX"))
                .ReturnsAsync((Country?)null);

            Func<Task> act = async () =>
                await _service.GetByIdAsync("XX");

            await act.Should()
                .ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AddAsync_ShouldCreateCountry_WhenValid()
        {
            var dto =
                CountryTestData.GetCountryDto();

            _repositoryMock
                .SetupSequence(x => x.GetByIdAsync(dto.CountryId!))
                .ReturnsAsync((Country?)null)
                .ReturnsAsync(
                    CountryTestData.GetCountryEntity());

            _regionRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.RegionId))
                .ReturnsAsync(new Region());

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Country>()))
                .ReturnsAsync(
                    CountryTestData.GetCountryEntity());

            var result =
                await _service.AddAsync(dto);

            result.Should().NotBeNull();

            result.CountryName.Should().Be("India");
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenDuplicateExists()
        {
            var dto =
                CountryTestData.GetCountryDto();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(dto.CountryId!))
                .ReturnsAsync(
                    CountryTestData.GetCountryEntity());

            Func<Task> act = async () =>
                await _service.AddAsync(dto);

            await act.Should()
                .ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenRegionNotFound()
        {
            var dto =
                CountryTestData.GetCountryDto();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(dto.CountryId!))
                .ReturnsAsync((Country?)null);

            _regionRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.RegionId))
                .ReturnsAsync((Region?)null);

            Func<Task> act = async () =>
                await _service.AddAsync(dto);

            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenIdMismatch()
        {
            var dto =
                CountryTestData.GetCountryDto();

            dto.CountryId = "US";

            Func<Task> act = async () =>
                await _service.UpdateAsync("IN", dto);

            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            _repositoryMock
                .Setup(x => x.GetByIdAsync("IN"))
                .ReturnsAsync(
                    CountryTestData.GetCountryEntity());

            _repositoryMock
                .Setup(x => x.DeleteAsync("IN"))
                .ReturnsAsync(true);

            var result =
                await _service.DeleteAsync("IN");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenNotFound()
        {
            _repositoryMock
                .Setup(x => x.GetByIdAsync("XX"))
                .ReturnsAsync((Country?)null);

            Func<Task> act = async () =>
                await _service.DeleteAsync("XX");

            await act.Should()
                .ThrowAsync<NotFoundException>();
        }
    }
}
