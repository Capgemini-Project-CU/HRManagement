using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Controllers
{
    public class RolesControllerTests
    {
        private readonly Mock<IRoleService> _mockService;

        private readonly RolesController _controller;

        public RolesControllerTests()
        {
            _mockService =
                new Mock<IRoleService>();

            _controller =
                new RolesController(
                    _mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult()
        {
            var roles = new List<RoleDto>
            {
                new RoleDto
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },

                new RoleDto
                {
                    RoleId = 4,
                    RoleName = "Manager"
                }
            };

            _mockService.Setup(s =>
                s.GetAllAsync())
                .ReturnsAsync(roles);

            var result =
                await _controller.GetAll();

            var okResult =
                Assert.IsType<OkObjectResult>(
                    result);

            var returnedRoles =
                Assert.IsAssignableFrom<
                    IEnumerable<RoleDto>>(
                    okResult.Value);

            Assert.Equal(
                2,
                returnedRoles.Count());
        }

        [Fact]
        public async Task GetById_ReturnsOkResult()
        {
            var role = new RoleDto
            {
                RoleId = 1,
                RoleName = "Admin"
            };

            _mockService.Setup(s =>
                s.GetByIdAsync(1))
                .ReturnsAsync(role);

            var result =
                await _controller.GetById(1);

            var okResult =
                Assert.IsType<OkObjectResult>(
                    result);

            var returnedRole =
                Assert.IsType<RoleDto>(
                    okResult.Value);

            Assert.Equal(
                "Admin",
                returnedRole.RoleName);
        }

        [Fact]
        public async Task Create_ReturnsOkResult()
        {
            var dto = new RoleDto
            {
                RoleId = 4,
                RoleName = "Manager"
            };

            _mockService.Setup(s =>
                s.CreateAsync(dto))
                .ReturnsAsync(dto);

            var result =
                await _controller.Create(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        [Fact]
        public async Task Update_ReturnsOkResult()
        {
            var dto = new RoleDto
            {
                RoleId = 4,
                RoleName = "Senior Manager"
            };

            _mockService.Setup(s =>
                s.UpdateAsync(4, dto))
                .ReturnsAsync(true);

            var result =
                await _controller.Update(
                    4,
                    dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult()
        {
            _mockService.Setup(s =>
                s.DeleteAsync(4))
                .ReturnsAsync(true);

            var result =
                await _controller.Delete(4);

            Assert.IsType<OkObjectResult>(
                result);
        }
    }
}