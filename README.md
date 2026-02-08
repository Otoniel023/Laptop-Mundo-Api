# ApiLaptopMundo - E-commerce Multi-tenant API

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)

A powerful, modern, and scalable Multi-tenant E-commerce API built with **.NET 10 Minimal APIs**, leveraging **Supabase** for authentication and storage, and **.NET Aspire** for cloud-ready orchestration.

---

## 🚀 Key Features

- **Multi-tenant Architecture**: Isolated data and configurations for different vendors/tenants.
- **Supabase Integration**:
  - Secure JWT-based Authentication.
  - Role-based Access Control (RBAC).
  - High-performance PostgreSQL database.
- **Clean Architecture**: Decoupled design following Domain-Driven Design (DDD) principles.
- **Admin Dashboard Ready**: Full set of endpoints for inventory, categories, and discounts management.
- **API Documentation**: Interactive documentation using **Scalar**.
- **Cloud Orchestration**: Managed with **.NET Aspire** for easy deployment and service discovery.

---

## 🛠️ Tech Stack

- **Framework**: .NET 10 Minimal APIs
- **Database & Auth**: [Supabase](https://supabase.com/)
- **Orchestration**: [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)
- **API Documentation**: [Scalar](https://scalar.com/)
- **Mapping & Patterns**: Custom Clean Architecture structure

---

## 📂 Project Structure

```bash
ApiLaptopMundo/
├── ApiLaptopMundo.AppHost/      # .NET Aspire Orchestrator
├── src/
│   ├── ApiLaptopMundo.Domain/         # Core entities, value objects, and domain logic
│   ├── ApiLaptopMundo.Application/    # DTOs, interfaces, and business application services
│   ├── ApiLaptopMundo.Infrastructure/ # Supabase services, external integrations
│   └── ApiLaptopMundo.WebApi/        # Minimal API endpoints, configurations, and middleware
└── database/                    # SQL scripts and schema documentation
```

---

## 🚦 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Aspire Workloads](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)
- A Supabase project

### Configuration

Update your `appsettings.json` or `appsettings.Development.json` in the `ApiLaptopMundo.WebApi` project:

```json
{
  "Supabase": {
    "Url": "YOUR_SUPABASE_URL",
    "Key": "YOUR_SUPABASE_ANON_KEY",
    "JwtSecret": "YOUR_SUPABASE_JWT_SECRET"
  }
}
```

### Running the Project

1. **Via .NET Aspire (Recommended)**:

   ```bash
   dotnet run --project ApiLaptopMundo.AppHost
   ```

   This will start the API along with the Aspire dashboard.

2. **Via WebApi project directly**:
   ```bash
   dotnet run --project src/ApiLaptopMundo.WebApi
   ```

---

## 📖 API Documentation

Once the API is running, you can access the interactive documentation at:

- **Scalar**: `http://localhost:<port>/scalar/v1`

---

## 🏗️ Architecture Detail

- **Domain**: Contains pure business logic, entities (Product, Category, Tenant), and domain-specific rules.
- **Application**: Orchestrates business flow using DTOs and handles the logic between Domain and Infrastructure.
- **Infrastructure**: Implementation of data persistence via Supabase client and external services like Email notifications.
- **WebApi**: The entry point, handling HTTP requests, auth middleware, and endpoint mapping.

---

## 🤝 Contributing

1. Fork the repository.
2. Create your feature branch (`git checkout -b feature/AmazingFeature`).
3. Commit your changes (`git commit -m 'Add AmazingFeature'`).
4. Push to the branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---

**Developed by members of the Laptop Mundo team.**
**Powered by Otoniel Martinez** - [otonielmartinez.com](https://otonielmartinez.com/)
