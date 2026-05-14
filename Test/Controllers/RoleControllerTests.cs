using HumanResource.API.Controllers;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.TestData;

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
        public async Task GetAll_ReturnsOkResult_WithRoles()
        {
            var roles = RoleTestData.GetRoles()
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToList();

            _mockService.Setup(s =>
                s.GetAllAsync())
                .ReturnsAsync(roles);

            var result =
                await _controller.GetAll();

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedRoles =
                Assert.IsAssignableFrom<
                    IEnumerable<RoleDto>>(
                        okResult.Value);

            Assert.Equal(3, returnedRoles.Count());
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsCorrectRole()
        {

            var dto = RoleTestData.GetRoleDto();

            _mockService.Setup(s =>
                s.GetByIdAsync(4))
                .ReturnsAsync(dto);

            var result =
                await _controller.GetById(4);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedRole =
                Assert.IsType<RoleDto>(
                    okResult.Value);

            Assert.Equal(
                "Manager",
                returnedRole.RoleName);
        }

        [Fact]
        public async Task Create_ValidRole_ReturnsOkResult()
        {
            var dto = RoleTestData.GetRoleDto();

            _mockService.Setup(s =>
                s.CreateAsync(dto))
                .ReturnsAsync(dto);

            var result =
                await _controller.Create(dto);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            var returnedRole =
                Assert.IsType<RoleDto>(
                    okResult.Value);

            Assert.Equal(
                dto.RoleName,
                returnedRole.RoleName);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsSuccessMessage()
        {

            _mockService.Setup(s =>
                s.DeleteAsync(4))
                .ReturnsAsync(true);

            var result =
                await _controller.Delete(4);

            var okResult =
                Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                "Role with Id 4 deleted successfully",
                okResult.Value);
        }

        [Fact]
        public async Task GetById_InvalidId_ThrowsNotFoundException()
        {

            _mockService.Setup(s =>
                s.GetByIdAsync(999))
                .ThrowsAsync(
                    new NotFoundException(
                        "Role not found"));

            await Assert.ThrowsAsync<
                NotFoundException>(() =>
                _controller.GetById(999));
        }

        [Fact]
        public async Task Create_DuplicateRole_ThrowsBadRequestException()
        {

            var dto = RoleTestData.GetRoleDto();

            _mockService.Setup(s =>
                s.CreateAsync(dto))
                .ThrowsAsync(
                    new BadRequestException(
                        "Role already exists"));

            await Assert.ThrowsAsync<
                BadRequestException>(() =>
                _controller.Create(dto));
        }

        [Fact]
        public async Task Update_InvalidId_ThrowsNotFoundException()
        {
            var dto =
                RoleTestData.GetUpdatedRoleDto();

            _mockService.Setup(s =>
                s.UpdateAsync(
                    999,
                    dto))
                .ThrowsAsync(
                    new NotFoundException(
                        "Role not found"));

            await Assert.ThrowsAsync<
                NotFoundException>(() =>
                _controller.Update(
                    999,
                    dto));
        }

        [Fact]
        public async Task Delete_InvalidId_ThrowsNotFoundException()
        {
            _mockService.Setup(s =>
                s.DeleteAsync(999))
                .ThrowsAsync(
                    new NotFoundException(
                        "Role not found"));


            await Assert.ThrowsAsync<
                NotFoundException>(() =>
                _controller.Delete(999));
        }
    }
}