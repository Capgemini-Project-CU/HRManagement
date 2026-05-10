using HumanResource.API.Data;
using Microsoft.EntityFrameworkCore;
using HumanResource.API.Mappings;
using FluentValidation;
using FluentValidation.AspNetCore;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Repositories.Implementations;
using HumanResource.API.Services.Interfaces;
using HumanResource.API.Services.Implementations;
using HumanResource.API.Middleware;

namespace HumanResource.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Database Connection
            builder.Services.AddDbContext<HRDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Fluent Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Repository Dependency Injection

            // Employee + JobHistory
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IJobHistoryRepository, JobHistoryRepository>();

            // Department
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            // Service Dependency Injection

            // Employee + JobHistory
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IJobHistoryService, JobHistoryService>();

            // Department
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Global Exception Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}