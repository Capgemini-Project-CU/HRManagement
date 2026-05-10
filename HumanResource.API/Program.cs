using FluentValidation;
using FluentValidation.AspNetCore;
using HumanResource.API.Data;
using HumanResource.API.Mappings;
using HumanResource.API.Repositories.Implementations;
using HumanResource.API.Repositories.Interfaces;
using HumanResource.API.Services.Implementations;
using HumanResource.API.Services.Interfaces;
using HumanResource.API.Validators;
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddDbContext<HRDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddValidatorsFromAssemblyContaining<LocationRequestValidator>();

            builder.Services.AddScoped<ILocationService, LocationService>();

            builder.Services.AddScoped<ILocationRepository, LocationRepository>();

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
