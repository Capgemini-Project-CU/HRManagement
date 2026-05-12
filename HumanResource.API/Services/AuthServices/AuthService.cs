using HumanResource.API.Authentication;
using HumanResource.API.DTOs.AuthDtos;
using HumanResource.API.Exceptions;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Implementations;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace HumanResource.API.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IAuthRepository repository, IOptions<JwtSettings> jwtOptions)
        {
            _repository = repository;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<string> RegisterAsync(
    RegisterRequestDto request)
        {
            var existingUser =
                await _repository.GetByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new Exception("Email already exists");
            }

            var lastEmployee =
    await _repository.GetLastEmployeeAsync();

            decimal nextEmployeeId =
                lastEmployee == null
                ? 207
                : lastEmployee.EmployeeId + 1;

            var employee = new Employee
            {
                EmployeeId = nextEmployeeId,

                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,

                RoleId = request.RoleId,

                HireDate = DateOnly.FromDateTime(DateTime.Now),

                JobId = request.JobId,
                Salary = request.Salary,
                DepartmentId = request.DepartmentId,
                ManagerId = request.ManagerId,

                IsActive = true
            };

            var passwordHasher =
                new PasswordHasher<Employee>();

            employee.PasswordHash =
                passwordHasher.HashPassword(employee, request.Password);

            await _repository.AddUserAsync(employee);

            return "User registered successfully";
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request)
        {
            var employee = await _repository
                .GetByEmailAsync(request.Email);

            if (employee == null)
            {
                throw new UnauthorizedException(
                    "Invalid email or password");
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    employee.EmployeeId.ToString()),

                new Claim(
                    ClaimTypes.Email, 
                    employee.Email),

                new Claim(
                    ClaimTypes.Role,
                    employee.Role!.RoleName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    _jwtSettings.ExpiryMinutes),
                signingCredentials: credentials);

            var jwtToken = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return new LoginResponseDto
            {
                Token = jwtToken,
                Email = employee.Email,
                Role = employee.Role.RoleName,
                Expiration = token.ValidTo
            };
        }
    }
}