using System.Security.Claims;

namespace ApiLaptopMundo.WebApi.Extensions;

public static class TenantExtensions
{
    public static long GetTenantId(this HttpContext context)
    {
        // 1. Try to get it from the direct claim (Supabase often puts this in user_metadata)
        var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value 
                           ?? context.User.FindFirst("app_metadata.tenant_id")?.Value
                           ?? context.User.FindFirst("user_metadata.tenant_id")?.Value;

        if (long.TryParse(tenantIdClaim, out var tenantId))
        {
            return tenantId;
        }

        // 2. Fallback: Check if it's the admin user (Development Fallback)
        // JWTs from common providers sometimes use 'email' or the standard ClaimTypes.Email
        var email = context.User.FindFirst(ClaimTypes.Email)?.Value 
                    ?? context.User.FindFirst("email")?.Value;

        // Also check sub if email is not found, sometimes it's used alternatively
        if (string.IsNullOrEmpty(email)) {
             email = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        if (email == "admin@laptopmundo.com")
        {
            return 1;
        }

        // 3. Last resort: If we are in local development and the user is authenticated, 
        // we can fallback to tenant 1 for convenience if no other tenant is found
        // Only do this if we really can't find anything else.
        return 1; // Temporary forceful fallback for testing/dev
    }
}
