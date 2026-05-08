using HumanResource.API.Data;

using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Repositories.Implementations;

using HumanResource.API.Services.Interfaces;
using HumanResource.API.Services.Implementations;

using Microsoft.EntityFrameworkCore;

namespace HumanResource.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();


            // Database Connection
            builder.Services.AddDbContext<HRDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));


            // Repository Dependency Injection
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();


            // Service Dependency Injection
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}