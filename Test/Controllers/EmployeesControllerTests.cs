using FluentAssertions;
using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.TestData;

namespace Test.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService>
            _serviceMock;

        private readonly EmployeesController
            _controller;

        public EmployeesControllerTests()
        {
            _serviceMock =
                new Mock<IEmployeeService>();

            _controller =
                new EmployeesController(
                    _serviceMock.Object);
        }

        // ---------------- POSITIVE TEST CASES ----------------

        [Fact]
        public async Task
            GetAllEmployees_ShouldReturnOk()
        {
            var data = new List<EmployeeDto>
            {
                EmployeeTestData.GetEmployeeDto()
            };

            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(data);

            var result =
                await _controller.GetAllEmployees();

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task
            GetById_ShouldReturnOk_WhenEmployeeExists()
        {
            var dto =
                EmployeeTestData.GetEmployeeDto();

            _serviceMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync(dto);

            var result =
                await _controller.GetEmployeeById(100);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task
            AddEmployee_ShouldReturnOk_WhenEmployeeCreated()
        {
            var dto =
                EmployeeTestData.GetEmployeeDto();

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ReturnsAsync(dto);

            var result =
                await _controller.AddEmployee(dto);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task
            DeleteEmployee_ShouldReturnOk_WhenDeleted()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(100))
                .ReturnsAsync(true);

            var result =
                await _controller.DeleteEmployee(100);

            result.Should()
                .BeOfType<OkObjectResult>();
        }

        // ---------------- NEGATIVE TEST CASES ----------------

        [Fact]
        public async Task
            GetById_ShouldThrowException_WhenEmployeeNotFound()
        {
            _serviceMock
                .Setup(x => x.GetByIdAsync(999))
                .ThrowsAsync(
                    new Exception("Employee not found"));

            Func<Task> act = async () =>
                await _controller.GetEmployeeById(999);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Employee not found");
        }

        [Fact]
        public async Task
            AddEmployee_ShouldThrowException_WhenInvalid()
        {
            var dto =
                EmployeeTestData.GetEmployeeDto();

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ThrowsAsync(
                    new Exception("Invalid employee"));

            Func<Task> act = async () =>
                await _controller.AddEmployee(dto);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Invalid employee");
        }

        [Fact]
        public async Task
            DeleteEmployee_ShouldThrowException_WhenNotFound()
        {
            _serviceMock
                .Setup(x => x.DeleteAsync(999))
                .ThrowsAsync(
                    new Exception("Employee not found"));

            Func<Task> act = async () =>
                await _controller.DeleteEmployee(999);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Employee not found");
        }

        [Fact]
        public async Task
            GetAllEmployees_ShouldThrowException_WhenFailed()
        {
            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ThrowsAsync(
                    new Exception("Database error"));

            Func<Task> act = async () =>
                await _controller.GetAllEmployees();

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Database error");
        }
    }
}