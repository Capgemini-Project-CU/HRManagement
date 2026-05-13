using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Test.Repositories
{
    public class RoleRepositoryTests
    {
        private HRDbContext GetDbContext()
        {
            var options =
                new DbContextOptionsBuilder<HRDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

            return new HRDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsRoles()
        {
            var context = GetDbContext();

            context.Roles.AddRange(
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },

                new Role
                {
                    RoleId = 4,
                    RoleName = "Manager"
                });

            await context.SaveChangesAsync();

            var repository =
                new RoleRepository(context);

            var result =
                await repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_AddsRole()
        {
            var context = GetDbContext();

            var repository =
                new RoleRepository(context);

            var role = new Role
            {
                RoleId = 4,
                RoleName = "Manager"
            };

            await repository.AddAsync(role);

            var addedRole =
                await context.Roles.FindAsync(4);

            Assert.NotNull(addedRole);
        }

        [Fact]
        public async Task DeleteAsync_RemovesRole()
        {
            var context = GetDbContext();

            var role = new Role
            {
                RoleId = 4,
                RoleName = "Manager"
            };

            context.Roles.Add(role);

            await context.SaveChangesAsync();

            var repository =
                new RoleRepository(context);

            await repository.DeleteAsync(role);

            var deletedRole =
                await context.Roles.FindAsync(4);

            Assert.Null(deletedRole);
        }
    }
}