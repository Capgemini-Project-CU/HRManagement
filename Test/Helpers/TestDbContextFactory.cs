using HumanResource.API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Helpers
{
    public static class TestDbContextFactory
    {
        public static HRDbContext CreateDbContext()
        {
            var options =
                new DbContextOptionsBuilder<HRDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new HRDbContext(options);
        }
    }
}
