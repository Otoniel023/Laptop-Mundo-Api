using ApiLaptopMundo.Infrastructure.Authentication;
using ApiLaptopMundo.Infrastructure.Services;
using ApiLaptopMundo.Application.Interfaces;
using ApiLaptopMundo.WebApi.Endpoints;
using Scalar.AspNetCore;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS to allow access from any domain (essential for generic multi-tenancy)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Allow any origin, useful for dynamic multi-tenancy
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10)); // Cache preflight results
    });
});

// Add Supabase Client
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "YOUR_SUPABASE_URL";
var supabaseKey = builder.Configuration["Supabase:Key"] ?? "YOUR_SUPABASE_ANON_KEY";
var jwtSecret = builder.Configuration["Supabase:JwtSecret"] ?? "YOUR_SUPABASE_JWT_SECRET";

builder.Services.AddScoped(_ => new Client(supabaseUrl, supabaseKey, new SupabaseOptions { AutoRefreshToken = true }));

// Add Auth
builder.Services.AddSupabaseAuth(jwtSecret);
builder.Services.AddAuthorization();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// Register Admin Services
builder.Services.AddScoped<IAdminProductService, AdminProductService>();
builder.Services.AddScoped<IAdminCategoryService, AdminCategoryService>();
builder.Services.AddScoped<IAdminDiscountService, AdminDiscountService>();
// TODO: Add more services as they are implemented

var app = builder.Build();

// Configure Scalar API Documentation
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("ApiLaptopMundo - E-commerce Multi-tenant API")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "ApiLaptopMundo is running! Visit /scalar/v1 for API documentation")
   .WithName("GetRoot")
   .WithTags("General")
   .WithDescription("Root endpoint to verify API is running");

app.MapGet("/api/test/auth", (System.Security.Claims.ClaimsPrincipal user) =>
    $"Hello {user.Identity?.Name ?? "Authenticated User"}! You are correctly authenticated with Supabase.")
   .RequireAuthorization()
   .WithName("TestAuth")
   .WithTags("Authentication")
   .WithDescription("Test endpoint to verify authentication is working");

app.MapPost("/api/auth/login", async (LoginRequest request, Client supabaseClient) =>
{
    try
    {
        var session = await supabaseClient.Auth.SignIn(request.Email, request.Password);
        
        // Try to get tenantId safely from metadata
        object? metadataTenantId = null;
        session?.User?.UserMetadata?.TryGetValue("tenant_id", out metadataTenantId);
        
        var userRole = session?.User?.UserMetadata?.GetValueOrDefault("role", "admin")?.ToString();

        // Fallback for development: if user is admin or specific user, and no tenantId assigned
        if (metadataTenantId == null) {
            if (userRole == "admin" || request.Email == "admin@laptopmundo.com" || request.Email == "otoniel16230@gmail.com") {
                metadataTenantId = 1;
            }
        }

        return Results.Ok(new { 
            access_token = session?.AccessToken,
            user = new {
                id = session?.User?.Id,
                email = session?.User?.Email,
                role = userRole,
                tenantId = metadataTenantId
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.WithName("Login")
.WithTags("Authentication")
.WithDescription("Login with email and password to get access token");

// Map Endpoints
app.MapProductEndpoints();
app.MapCategoryEndpoints();
app.MapTenantEndpoints();

// Map Public Endpoints (no authentication required)
app.MapPublicTenantEndpoints();
app.MapPublicProductEndpoints();

// Map Admin Endpoints
app.MapAdminProductEndpoints();
app.MapAdminCategoryEndpoints();
app.MapAdminDiscountEndpoints();

app.Run();

public record LoginRequest(string Email, string Password, bool RememberMe = false);


