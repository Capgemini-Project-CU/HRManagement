using Moq;
using Test.Helpers;
using Test.TestData;
using AutoMapper;
using FluentAssertions;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;


namespace Test.Services
{
    public class LocationServiceTests
    {
        private readonly Mock<ILocationRepository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly LocationService _service;

        public LocationServiceTests()
        {
            _repositoryMock = new Mock<ILocationRepository>();
            _mapper = TestUtilities.GetMapper();
            _service = new LocationService( _repositoryMock.Object,_mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLocation_WhenExists()
        {
            var location = LocationTestData.GetLocationEntity();
            _repositoryMock.Setup(x => x.GetByIdAsync(1000)).ReturnsAsync(location);
            var result = await _service.GetByIdAsync(1000);
            result.Should().NotBeNull();
            result!.LocationId.Should().Be(1000);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateLocation_WhenValid()
        {
            var dto = LocationTestData.GetLocationRequestDto();

            _repositoryMock
                .SetupSequence(x => x.GetByIdAsync(dto.LocationId))
                .ReturnsAsync((Location?)null)
                .ReturnsAsync(LocationTestData.GetLocationEntity());

            _repositoryMock
                .Setup(x => x.CountryExistsAsync(dto.CountryId))
                .ReturnsAsync(true);

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Location>()))
                .ReturnsAsync(LocationTestData.GetLocationEntity());

            var result =await _service.CreateAsync(dto);
            result.Should().NotBeNull();
            result.City.Should().Be("Chandigarh");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenLocationNotFound()
        {
            _repositoryMock
                .Setup(x => x.GetByIdAsync(9999))
                .ReturnsAsync((Location?)null);

            Func<Task> act = async () => await _service.GetByIdAsync(9999);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCountryInvalid()
        {
            var dto = LocationTestData.GetLocationRequestDto();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(dto.LocationId))
                .ReturnsAsync((Location?)null);

            _repositoryMock
                .Setup(x => x.CountryExistsAsync(dto.CountryId))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _service.CreateAsync(dto);
            await act.Should().ThrowAsync<BadRequestException>();
        }
    }
}
