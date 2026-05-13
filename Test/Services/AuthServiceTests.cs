using HumanResource.API.Authentication;
using HumanResource.API.DTOs.AuthDtos;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.AuthServices;
using Microsoft.Extensions.Options;
using Moq;

namespace Test.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockRepo;

        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockRepo =
                new Mock<IAuthRepository>();

            var jwtSettings = Options.Create(
                new JwtSettings
                {
                    Key =
                    "HumanResourceAPIJwtAuthenticationSecretKey2026CapgeminiProjectVerySecureEncryptionKey",

                    Issuer =
                    "HumanResource.API",

                    Audience =
                    "HumanResource.API.Client",

                    ExpiryMinutes = 240
                });

            _service = new AuthService(
                _mockRepo.Object,
                jwtSettings);
        }

        [Fact]
        public async Task RegisterAsync_ValidUser_ReturnsSuccessMessage()
        {
            // Arrange

            var request = new RegisterRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password123",
                PhoneNumber = "1234567890",
                JobId = "IT_PROG",
                Salary = 5000,
                DepartmentId = 1,
                ManagerId = null,
                RoleId = 1
            };

            _mockRepo.Setup(r =>
                r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Employee?)null);

            _mockRepo.Setup(r =>
                r.GetLastEmployeeAsync())
                .ReturnsAsync((Employee?)null);

            _mockRepo.Setup(r =>
                r.AddUserAsync(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            // Act

            var result =
                await _service.RegisterAsync(
                    request);

            // Assert

            Assert.Equal(
                "User registered successfully",
                result);

            _mockRepo.Verify(
                r => r.AddUserAsync(
                    It.IsAny<Employee>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateEmail_ThrowsException()
        {
            // Arrange

            var request = new RegisterRequestDto
            {
                Email = "john@example.com"
            };

            var existingUser = new Employee
            {
                EmployeeId = 207,
                Email = "john@example.com"
            };

            _mockRepo.Setup(r =>
                r.GetByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(
                () => _service.RegisterAsync(
                    request));
        }

        [Fact]
        public async Task LoginAsync_ValidUser_ReturnsToken()
        {
            // Arrange

            var request = new LoginRequestDto
            {
                Email = "john@example.com",
                Password = "Password123"
            };

            var employee = new Employee
            {
                EmployeeId = 207,
                Email = "john@example.com",

                Role = new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                }
            };

            _mockRepo.Setup(r =>
                r.GetByEmailAsync(request.Email))
                .ReturnsAsync(employee);

            // Act

            var result =
                await _service.LoginAsync(
                    request);

            // Assert

            Assert.NotNull(result);

            Assert.NotNull(result.Token);

            Assert.Equal(
                "john@example.com",
                result.Email);

            Assert.Equal(
                "Admin",
                result.Role);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ThrowsUnauthorizedException()
        {
            // Arrange

            var request = new LoginRequestDto
            {
                Email = "invalid@example.com",
                Password = "Password123"
            };

            _mockRepo.Setup(r =>
                r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Employee?)null);

            // Act & Assert

            await Assert.ThrowsAsync<
                UnauthorizedException>(
                () => _service.LoginAsync(
                    request));
        }

        [Fact]
        public async Task LoginAsync_ReturnsValidExpirationTime()
        {
            // Arrange

            var request = new LoginRequestDto
            {
                Email = "john@example.com",
                Password = "Password123"
            };

            var employee = new Employee
            {
                EmployeeId = 207,
                Email = "john@example.com",

                Role = new Role
                {
                    RoleId = 1,
                    RoleName = "HR"
                }
            };

            _mockRepo.Setup(r =>
                r.GetByEmailAsync(request.Email))
                .ReturnsAsync(employee);

            // Act

            var result =
                await _service.LoginAsync(
                    request);

            // Assert

            Assert.True(
                result.Expiration > DateTime.UtcNow);
        }

        [Fact]
        public async Task LoginAsync_ReturnsCorrectRole()
        {
            // Arrange

            var request = new LoginRequestDto
            {
                Email = "manager@example.com",
                Password = "Password123"
            };

            var employee = new Employee
            {
                EmployeeId = 300,
                Email = "manager@example.com",

                Role = new Role
                {
                    RoleId = 2,
                    RoleName = "Manager"
                }
            };

            _mockRepo.Setup(r =>
                r.GetByEmailAsync(request.Email))
                .ReturnsAsync(employee);

            // Act

            var result =
                await _service.LoginAsync(
                    request);

            // Assert

            Assert.Equal(
                "Manager",
                result.Role);
        }
    }
}