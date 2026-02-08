namespace ApiLaptopMundo.Application.DTOs.Common;

public record PaginatedResponseDto<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);

public record ApiResponseDto<T>(
    bool Success,
    T? Data,
    string? Message,
    List<string>? Errors
);
