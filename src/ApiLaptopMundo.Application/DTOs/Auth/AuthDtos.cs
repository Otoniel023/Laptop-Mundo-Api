namespace ApiLaptopMundo.Application.DTOs.Auth;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(string AccessToken, string RefreshToken, UserDto User);

public record RegisterRequestDto(
    string Email,
    string Password,
    string Username,
    long TenantId
);

public record RefreshTokenRequestDto(string RefreshToken);

public record UserDto(
    long Id,
    string Username,
    string Email,
    long TenantId,
    DateTime CreatedAt
);
