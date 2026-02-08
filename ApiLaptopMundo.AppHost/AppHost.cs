using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//Add API LaptopMundo
var api = builder.AddProject("ApiLaptopMundo", "../src/ApiLaptopMundo.WebApi/ApiLaptopMundo.WebApi.csproj");

//Add Frontend LaptopMundo
var laptopMundo = builder.AddViteApp("LaptopMundo", "../../LaptopMundo")
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("https"));

builder.Build().Run();
