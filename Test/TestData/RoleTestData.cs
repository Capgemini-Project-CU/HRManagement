using HumanResource.API.DTOs;
using HumanResource.API.Models;

namespace Test.TestData
{
    public static class RoleTestData
    {
        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },

                new Role
                {
                    RoleId = 2,
                    RoleName = "HR"
                },

                new Role
                {
                    RoleId = 4,
                    RoleName = "Manager"
                }
            };
        }

        public static Role GetRole()
        {
            return new Role
            {
                RoleId = 1,
                RoleName = "Admin"
            };
        }

        public static RoleDto GetRoleDto()
        {
            return new RoleDto
            {
                RoleId = 4,
                RoleName = "Manager"
            };
        }

        public static RoleDto GetUpdatedRoleDto()
        {
            return new RoleDto
            {
                RoleId = 4,
                RoleName = "Senior Manager"
            };
        }
    }
}