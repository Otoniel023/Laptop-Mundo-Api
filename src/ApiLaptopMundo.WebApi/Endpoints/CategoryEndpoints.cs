using ApiLaptopMundo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ApiLaptopMundo.WebApi.Extensions;

namespace ApiLaptopMundo.WebApi.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        // GET /api/categories - List all categories
        group.MapGet("/", async ([FromServices] ICategoryService categoryService) =>
        {
            var categories = await categoryService.GetCategoriesAsync();
            return Results.Ok(categories);
        })
        .WithName("GetCategories")
        .WithDescription("Get a list of all product categories (Gaming, Ultrabook, Workstation, etc.)")
        .WithSummary("List all categories");

        // GET /api/categories/{id} - Get category by ID
        group.MapGet("/{id:long}", async (
            long id,
            [FromServices] ICategoryService categoryService) =>
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            return category != null ? Results.Ok(category) : Results.NotFound();
        })
        .WithName("GetCategoryById")
        .WithDescription("Get detailed information about a specific category")
        .WithSummary("Get category details");

        // GET /api/categories/{id}/products - Get products by category
        group.MapGet("/{id:long}/products", async (
            long id,
            [FromServices] ICategoryService categoryService,
            HttpContext context) =>
        {
            var tenantId = context.GetTenantId();
            var products = await categoryService.GetProductsByCategoryAsync(tenantId, id);
            return Results.Ok(products);
        })
        .WithName("GetProductsByCategory")
        .WithDescription("Get all products that belong to a specific category")
        .WithSummary("Get products by category");
    }
}
