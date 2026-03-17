# Teachio API

A robust, scalable, and maintainable **ASP.NET Code Web API (C#)** for the LMS Teachio platform.  
This repository contains the backend service responsible for business logic, data access, and external integrations required by Teachio clients.

> **Status:** Active development

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Configuration](#configuration)
  - [Run the API](#run-the-api)
- [Environment Variables](#environment-variables)
- [Database](#database)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Code Style & Quality](#code-style--quality)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## Overview

**teachio-api** is the backend API layer for the Teachio ecosystem.  
It is designed to provide secure, versionable, and testable endpoints for LMS clients.

Typical responsibilities include:

- User and role management
- Authentication and authorization
- Domain/business operations
- Data persistence
- Validation and error handling
- Integrations with external systems

---

## Key Features

- Clean RESTful API monolith design
- N-Layer architecture (Web API Layer / Business Logic Layer / Data Access Layer)
- JWT-based authentication & authorization
- DTO mapping and request validation
- Centralized exception handling and structured logging
- Database migrations support
- OpenAPI/Swagger support
- Unit and integration testing support
- Environment-based configuration (Development / Staging / Production)

---

## Tech Stack

- **Language:** C# 
- **Framework:** ASP.NET Core
- **Runtime:** .NET 9 
- **ORM/Data Access:** Entity Framework Core
- **DBMS**: MS SQL Server
- **Requests handling**: MediatR, AutoMapper, FluentResults 
- **Logging**: Serilog 
- **Documentation:** Swagger / OpenAPI 
- **Testing:** xUnit, WebApplicationFatory, Moq, FluetAssertions, RestSharp

---

## Architecture

The project follows clean and maintainable backend practices:

- **Web API Layer (WebApi)** — Controllers, endpoint definitions, middlewares 
- **Data Access Layer (DAL)** — Database, models, repositories 
- **Business Logic Layer (BLL)** — Use cases, application services, DTOs 

This separation improves testability, readability, and long-term maintainability.

---

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) >= 9 (LTS version recommended)
- A configured database server
- Git

### Installation

```bash
git clone https://github.com/Michael-Kolpakov/teachio-api.git
cd teachio-api
dotnet restore
```

### Configuration

1. Create environment-specific configuration:
   - `appsettings.Development.json`
   - `appsettings.Staging.json`
   - `appsettings.Production.json`

2. Configure required values:
   - Connection strings
   - JWT settings
   - External service credentials
   - Logging options

3. (Optional) Use environment variables or user secrets for sensitive data.

### Run the API

```bash
dotnet build
dotnet run
```

By default, the API will start on configured HTTP/HTTPS ports (see launch settings or runtime logs).

---

## Environment Variables

Below is an example list of common variables used in ASP.NET APIs:

| Variable | Description | Example |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
| `ConnectionStrings__DefaultConnection` | Primary database connection string | `Server=...;Database=...;` |
| `Jwt__Issuer` | JWT token issuer | `teachio-api` |
| `Jwt__Audience` | JWT token audience | `teachio-clients` |
| `Jwt__Key` | JWT signing key | `your-very-strong-secret` |
| `Logging__LogLevel__Default` | Default log level | `Information` |

---

## Database

Project uses EF Core migrations:

```bash
dotnet ef database update --project "Teachio.DAL" --startup-project "Teachio.WebApi"
```

To create a new migration:

```bash
dotnet ef migrations add <MigrationName> --project "Teachio.DAL" --startup-project "Teachio.WebApi"
dotnet ef database update --project "Teachio.DAL" --startup-project "Teachio.WebApi"
```

Make sure the correct startup project and connection string are configured before running commands.

---

## API Documentation

For Swagger open:

- `https://localhost:<port>/swagger`
- or `http://localhost:<port>/swagger`

Use this UI to explore endpoints, request/response schemas, and test operations.

---

## Testing

Run all tests:

```bash
dotnet test
```

Recommended test categories:

- **Unit tests** for business logic
- **Integration tests** for API + infrastructure behavior

---

## Code Style & Quality

Recommended practices:

- Enabled nullable reference types
- Use analyzers and treat warnings seriously
- Keep controllers thin; move logic to services/use-cases
- Validate incoming DTOs
- Return standardized error responses (used Results Pattern via FluentResults library + standard error handling middleware)
- Write tests for critical business paths

---

## Project Structure

Example structure (adjust to your actual layout):

```text
teachio-api/
├─ Teachio.BLL/
│  ├─ Dto/
│  │  ├─ Courses/
│  │  │  ├─ Courses/
│  │  │  │  ├─ Request/
│  │  │  │  │  ├─ Create/
│  │  │  │  │  └─ Update/
│  │  │  │  └─ Response/
│  │  │  ├─ Sections/
│  │  │  │  ├─ Request/
│  │  │  │  │  ├─ Create/
│  │  │  │  │  └─ Update/
│  │  │  │  └─ Response/
│  │  │  └─ Videos/
│  │  │     ├─ VideoProgress/
│  │  │     │  ├─ Request/
│  │  │     │  │  └─ Update/
│  │  │     │  └─ Response/
│  │  │     └─ Videos/
│  │  │        ├─ Request/
│  │  │        │  ├─ Create/
│  │  │        │  └─ Update/
│  │  │        └─ Response/
│  │  ├─ Shared/
│  │  └─ Users/
│  │     ├─ Request/
│  │     │  ├─ Create/
│  │     │  └─ Update/
│  │     └─ Response/
│  ├─ Mapping/
│  │  ├─ Courses/
│  │  │  ├─ Course/
│  │  │  ├─ Section/
│  │  │  └─ Video/
│  │  │     ├─ Video/
│  │  │     └─ VideoProgress/
│  │  └─ User/
│  ├─ MediatR/
│  │  ├─ Courses/
│  │  │  ├─ Courses/
│  │  │  │  ├─ Create/
│  │  │  │  ├─ Delete/
│  │  │  │  ├─ GetById/
│  │  │  │  ├─ GetByIdPreview/
│  │  │  │  ├─ GetPaginated/
│  │  │  │  ├─ Udpate/
│  │  │  │  └─ UploadThumbnail/
│  │  │  ├─ Sections/
│  │  │  │  ├─ Create/
│  │  │  │  ├─ Delete/
│  │  │  │  ├─ GetById/
│  │  │  │  └─ Udpate/
│  │  │  └─ Videos/
│  │  │     ├─ VideoProgress/
│  │  │     │  └─ Update/
│  │  │     └─ Videos/
│  │  │        ├─ Create/
│  │  │        ├─ Delete/
│  │  │        ├─ GetById/
│  │  │        ├─ Update/
│  │  │        └─ UploadVideo/
│  │  └─ ResultValidations/
│  ├─ Models/
│  ├─ Resources/
│  ├─ Services/
│  │  ├─ Interfaces/
│  │  └─ Realizations/
│  ├─ SharedResource/
│  └─ Utils/
│     ├─ Constants/
│     └─ MappingResolvers/ 
├─ Teachio.DAL/
│  ├─ Entities/
│  │  ├─ Courses/
│  │  │  ├─ Courses/
│  │  │  ├─ Sections/
│  │  │  └─ Videos/
│  │  │     ├─ VideoProgress/
│  │  │     └─ Videos/
│  │  ├─ Shared/
│  │  └─ Users/
│  ├─ Persistence/
│  │  ├─ Migrations/
│  │  └─ Seed/
│  │     └─ Content/
│  ├─ Repositories/
│  │  ├─ Interfaces/
│  │  │  ├─ Base/
│  │  │  ├─ Courses/
│  │  │  │  ├─ Courses/
│  │  │  │  ├─ Sections/
│  │  │  │  └─ Videos/
│  │  │  │     ├─ VideoProgress/
│  │  │  │     └─ Videos/
│  │  │  └─ Users/
│  │  └─ Realizations/
│  │     ├─ Base/
│  │     ├─ Courses/
│  │     │  ├─ Courses/
│  │     │  ├─ Sections/
│  │     │  └─ Videos/
│  │     │     ├─ VideoProgress/
│  │     │     └─ Videos/
│  │     └─ Users/
│  ├─ Resources/
│  ├─ SharedResource/
│  └─ Utils/
│     ├─ Constants/
│     ├─ Database/
│     ├─ Helpers/
│     └─ Validators/
├─ Teachio.WebApi/
│  ├─ Controllers/
│  │  └─ Courses/
│  │     ├─ Courses/
│  │     ├─ Sections/
│  │     └─ Videos/
│  │        ├─ VideoProgress/
│  │        └─ Videos/
│  ├─ Extensions/
│  ├─ Middlewares/
│  ├─ Properties/
│  └─ Utils/
│     └─ RelativeRoutes/
├─ Teachio.IntegrationTests/
│  ├─ Base/
│  ├─ ControllerTests/
│  ├─ TestData/
│  └─ Utils/
│     ├─ BeforeAndAfterAttributes/
│     │  └─ Courses/
│     │  ├─ Sections/
│     │  └─ Videos/
│     │     ├─ VideoProgress/
│     │     └─ Videos/
│     ├─ Clients/
│     ├─ Extractors/
│     └─ Helpers/
└─ Teachio.UnitTests/
   ├─ MediatR/
   │  ├─ Courses/
   │  ├─ Sections/
   │  └─ Videos/
   │     ├─ VideoProgress/
   │     └─ Videos/
   ├─ Mocks/
   │  └─ Localizers/
   ├─ TestData/
   └─ Verifications/
```

---

## Contributing

Contributions are welcome.

1. Fork the repository
2. Create a feature branch:
   ```bash
   git checkout -b main-feature/your-feature-name
   ```
3. Commit changes:
   ```bash
   git commit -m "feat: add your feature"
   ```
4. Push branch and open a Pull Request

Please follow conventional commits and keep PRs focused.

---

## License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details..

---

## Contact

Maintainer: **Michael-Kolpakov**  
Repository: https://github.com/Michael-Kolpakov/teachio-api

For support, open an issue in this repository.
