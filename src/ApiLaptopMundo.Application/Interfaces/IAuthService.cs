using ApiLaptopMundo.Application.DTOs.Auth;

namespace ApiLaptopMundo.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<UserDto> GetCurrentUserAsync(string userId);
}
