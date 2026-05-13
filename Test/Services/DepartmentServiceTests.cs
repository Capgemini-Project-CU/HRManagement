using AutoMapper;
using FluentAssertions;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Mappings;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;
using Moq;
using Test.TestData;

namespace Test.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _repositoryMock;

        private readonly IMapper _mapper;

        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _repositoryMock = new Mock<IDepartmentRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = mapperConfig.CreateMapper();

            _service = new DepartmentService(
                _repositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnDepartments()
        {
            var departments = new List<Department>
            {
                new Department
                {
                    DepartmentId = 10,
                    DepartmentName = "IT"
                }
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(departments);

            var result = await _service.GetAllAsync();

            result.Should().NotBeNull();

            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDepartment_WhenDepartmentExists()
        {
            var department = new Department
            {
                DepartmentId = 10,
                DepartmentName = "IT"
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(department);

            var result = await _service.GetByIdAsync(10);

            result.Should().NotBeNull();

            result.DepartmentName.Should().Be("IT");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenDepartmentNotFound()
        {
            _repositoryMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync((Department?)null);

            Func<Task> act = async () =>
                await _service.GetByIdAsync(100);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Department not found");
        }

        [Fact]
        public async Task GetByLocationAsync_ShouldReturnDepartments()
        {
            var departments = new List<Department>
            {
                new Department
                {
                    DepartmentId = 10,
                    DepartmentName = "IT",
                    LocationId = 1700
                }
            };

            _repositoryMock
                .Setup(x => x.GetByLocationAsync(1700))
                .ReturnsAsync(departments);

            var result = await _service.GetByLocationAsync(1700);

            result.Should().NotBeNull();

            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetByLocationAsync_ShouldThrowException_WhenDepartmentsNotFound()
        {
            _repositoryMock
                .Setup(x => x.GetByLocationAsync(999))
                .ReturnsAsync(new List<Department>());

            Func<Task> act = async () =>
                await _service.GetByLocationAsync(999);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("No departments found");
        }

        [Fact]
        public async Task AddAsync_ShouldAddDepartment()
        {
            var dto = DepartmentTestData.GetDepartmentDto();

            var department = new Department
            {
                DepartmentId = dto.DepartmentId,
                DepartmentName = dto.DepartmentName,
                ManagerId = dto.ManagerId,
                LocationId = dto.LocationId
            };

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Department>()))
                .ReturnsAsync(department);

            var result = await _service.AddAsync(dto);

            result.Should().NotBeNull();

            result.DepartmentName.Should().Be(dto.DepartmentName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDepartment()
        {
            var dto = DepartmentTestData.GetDepartmentDto();

            var existingDepartment = new Department
            {
                DepartmentId = 10,
                DepartmentName = "IT"
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(existingDepartment);

            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Department>()))
                .ReturnsAsync(existingDepartment);

            var result = await _service.UpdateAsync(10, dto);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenDepartmentNotFound()
        {
            var dto = DepartmentTestData.GetDepartmentDto();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync((Department?)null);

            Func<Task> act = async () =>
                await _service.UpdateAsync(100, dto);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Department not found");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDepartmentDeleted()
        {
            _repositoryMock
                .Setup(x => x.DeleteAsync(10))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(10);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenDepartmentNotFound()
        {
            _repositoryMock
                .Setup(x => x.DeleteAsync(100))
                .ReturnsAsync(false);

            Func<Task> act = async () =>
                await _service.DeleteAsync(100);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Department not found");
        }
    }
}