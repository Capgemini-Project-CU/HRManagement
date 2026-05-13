using AutoMapper;
using HumanResource.API.DTOs;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;
using Test.TestData;
using Moq;

namespace Test.Services
{
    public class RoleServiceTests
    {
        private readonly Mock<IRoleRepository> _mockRepo;

        private readonly Mock<IMapper> _mockMapper;

        private readonly RoleService _service;

        public RoleServiceTests()
        {
            _mockRepo =
                new Mock<IRoleRepository>();

            _mockMapper =
                new Mock<IMapper>();

            _service = new RoleService(
                _mockRepo.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllRoles()
        {
            var roles = RoleTestData.GetRoles();

            var roleDtos = new List<RoleDto>
            {
                new RoleDto
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },

                new RoleDto
                {
                    RoleId = 2,
                    RoleName = "HR"
                }
            };

            _mockRepo.Setup(r =>
                r.GetAllAsync())
                .ReturnsAsync(roles);

            _mockMapper.Setup(m =>
                m.Map<IEnumerable<RoleDto>>(roles))
                .Returns(roleDtos);

            var result =
                await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsRole()
        {
            var role = RoleTestData.GetRole();

            var dto = new RoleDto
            {
                RoleId = 1,
                RoleName = "Admin"
            };

            _mockRepo.Setup(r =>
                r.GetByIdAsync(1))
                .ReturnsAsync(role);

            _mockMapper.Setup(m =>
                m.Map<RoleDto>(role))
                .Returns(dto);

            var result =
                await _service.GetByIdAsync(1);

            Assert.NotNull(result);

            Assert.Equal(
                "Admin",
                result.RoleName);
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsException()
        {
            _mockRepo.Setup(r =>
                r.GetByIdAsync(999))
                .ReturnsAsync((Role?)null);

            await Assert.ThrowsAsync<
                NotFoundException>(
                () => _service.GetByIdAsync(999));
        }

        [Fact]
        public async Task CreateAsync_ValidRole_ReturnsRole()
        {
            var dto =
                RoleTestData.GetRoleDto();

            var role = new Role
            {
                RoleId = dto.RoleId,
                RoleName = dto.RoleName
            };

            _mockMapper.Setup(m =>
                m.Map<Role>(dto))
                .Returns(role);

            _mockRepo.Setup(r =>
                r.AddAsync(It.IsAny<Role>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m =>
                m.Map<RoleDto>(role))
                .Returns(dto);

            var result =
                await _service.CreateAsync(dto);

            Assert.NotNull(result);

            Assert.Equal(
                "Manager",
                result.RoleName);
        }

        [Fact]
        public async Task UpdateAsync_ValidData_ReturnsTrue()
        {
            var role =
                RoleTestData.GetRole();

            var dto =
                RoleTestData.GetUpdatedRoleDto();

            _mockRepo.Setup(r =>
                r.GetByIdAsync(1))
                .ReturnsAsync(role);

            _mockRepo.Setup(r =>
                r.UpdateAsync(role))
                .Returns(Task.CompletedTask);

            var result =
                await _service.UpdateAsync(
                    1,
                    dto);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_InvalidId_ThrowsException()
        {
            var dto =
                RoleTestData.GetUpdatedRoleDto();

            _mockRepo.Setup(r =>
                r.GetByIdAsync(999))
                .ReturnsAsync((Role?)null);

            await Assert.ThrowsAsync<
                NotFoundException>(
                () => _service.UpdateAsync(
                    999,
                    dto));
        }

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsTrue()
        {
            var role =
                RoleTestData.GetRole();

            _mockRepo.Setup(r =>
                r.GetByIdAsync(1))
                .ReturnsAsync(role);

            _mockRepo.Setup(r =>
                r.DeleteAsync(role))
                .Returns(Task.CompletedTask);

            var result =
                await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsException()
        {
            _mockRepo.Setup(r =>
                r.GetByIdAsync(999))
                .ReturnsAsync((Role?)null);

            await Assert.ThrowsAsync<
                NotFoundException>(
                () => _service.DeleteAsync(999));
        }
    }
}