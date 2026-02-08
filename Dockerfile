# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file and project files first to leverage Docker cache
COPY ["ApiLaptopMundo.sln", "./"]
COPY ["src/ApiLaptopMundo.WebApi/ApiLaptopMundo.WebApi.csproj", "src/ApiLaptopMundo.WebApi/"]
COPY ["src/ApiLaptopMundo.Application/ApiLaptopMundo.Application.csproj", "src/ApiLaptopMundo.Application/"]
COPY ["src/ApiLaptopMundo.Domain/ApiLaptopMundo.Domain.csproj", "src/ApiLaptopMundo.Domain/"]
COPY ["src/ApiLaptopMundo.Infrastructure/ApiLaptopMundo.Infrastructure.csproj", "src/ApiLaptopMundo.Infrastructure/"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/ApiLaptopMundo.WebApi"
RUN dotnet build "ApiLaptopMundo.WebApi.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "ApiLaptopMundo.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "ApiLaptopMundo.WebApi.dll"]
