using AutoMapper;
using FluentAssertions;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;
using Moq;
using Test.Helpers;
using Test.TestData;

namespace Test.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository>
            _repositoryMock;

        private readonly IMapper _mapper;

        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _repositoryMock =
                new Mock<IEmployeeRepository>();

            _mapper =
                TestUtilities.GetMapper();

            _service =
                new EmployeeService(
                    _repositoryMock.Object,
                    _mapper);
        }

        [Fact]
        public async Task
            GetByIdAsync_ShouldReturnEmployee_WhenExists()
        {

            var employee =
                EmployeeTestData.GetEmployeeEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync(employee);

            var result =
                await _service.GetByIdAsync(100);


            result.Should().NotBeNull();

            result.EmployeeId.Should().Be(100);

            result.FirstName.Should().Be("Steven");
        }
        [Fact]
        public async Task
    GetByIdAsync_ShouldThrowException_WhenNotFound()
        {

            _repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((HumanResource.API.Models.Employee)null);


            Func<Task> act = async () =>
                await _service.GetByIdAsync(999);


            await act.Should()
                .ThrowAsync<HumanResource.API.Exceptions.NotFoundException>();
        }

        [Fact]
        public async Task
    AddAsync_ShouldCreateEmployee_WhenValid()
        {

            var employeeDto =
                EmployeeTestData.GetEmployeeDto();

            var employeeEntity =
                EmployeeTestData.GetEmployeeEntity();

            _repositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<HumanResource.API.Models.Employee>()))
                .ReturnsAsync(employeeEntity);


            var result =
                await _service.AddAsync(employeeDto);



            result.Should().NotBeNull();

            result.EmployeeId.Should().Be(100);

            result.FirstName.Should().Be("Steven");
        }

        [Fact]
        public async Task
    DeleteAsync_ShouldThrowException_WhenEmployeeNotFound()
        {

            _repositoryMock
                .Setup(x => x.DeleteAsync(999))
                .ReturnsAsync(false);


            Func<Task> act = async () =>
                await _service.DeleteAsync(999);


            await act.Should()
                .ThrowAsync<HumanResource.API.Exceptions.NotFoundException>();
        }
    }
}