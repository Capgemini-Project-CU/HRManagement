using HumanResource.API.DTOs.AuthDtos;

namespace HumanResource.API.Services.AuthServices
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request);
        Task<string> RegisterAsync(
            RegisterRequestDto request);
    }
}