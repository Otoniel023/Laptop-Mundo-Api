namespace ApiLaptopMundo.Application.DTOs.Admin;

// Tenant DTOs
public record CreateTenantDto(
    string Name,
    string? Domain,
    string? Description,
    string? BusinessType = null,
    object? Settings = null
);

public record UpdateTenantDto(
    string Name,
    string? Domain,
    string? Description,
    string? BusinessType = null,
    bool IsActive = true,
    object? Settings = null
);

public record TenantDto(
    long Id,
    string Name,
    string? Domain,
    string? Description,
    string? BusinessType,
    bool IsActive,
    object? Settings,
    DateTime CreatedAt
);

// User DTOs for Tenant
public record CreateTenantUserDto(
    string Email,
    string Password,
    string Role = "admin" // admin or customer
);

public record TenantUserDto(
    string Id,
    long TenantId,
    string Email,
    string Role,
    DateTime CreatedAt
);
