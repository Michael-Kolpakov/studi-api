# Teachio API

A robust, scalable, and maintainable **ASP.NET Core Web API (C#)** for the LMS Teachio platform.  
This repository contains the backend service responsible for business logic, data access, and external integrations required by Teachio clients.

> **Status:** Active development

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
  - [Developer Quick Start (Fresh Machine)](#developer-quick-start-fresh-machine)
- [Environment Variables](#environment-variables)
- [Database](#database)
- [API Documentation](#api-documentation)
- [Production Deployment Guide](#production-deployment-guide)
- [Backup and Restore Runbook](#backup-and-restore-runbook)
- [DevOps Automation Pack](#devops-automation-pack)
- [Documentation Standards](#documentation-standards)
- [Public API Inventory](#public-api-inventory)
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
- Centralized exception handling and structured logging
- Authentication/authorization flow is currently in progress
- DTO mapping and request validation
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
- **Testing:** xUnit, WebApplicationFactory, Moq, FluentAssertions, RestSharp

---

## Architecture

The project follows clean and maintainable backend practices:

- **Web API Layer (WebApi)** — Controllers, endpoint definitions, middlewares
- **Data Access Layer (DAL)** — Database, models, repositories
- **Business Logic Layer (BLL)** — Use cases, application services, DTOs

This separation improves testability, readability, and long-term maintainability.

---

## Getting Started

### Developer Quick Start (Fresh Machine)

This is a short end-to-end onboarding flow for a developer with a newly installed OS.

1. Install required software
   - [Git](https://git-scm.com/downloads)
   - [.NET SDK 9](https://dotnet.microsoft.com/download)
   - [SQL Server Developer or Express](https://www.microsoft.com/sql-server/sql-server-downloads)
   - [PowerShell 7+](https://learn.microsoft.com/powershell/scripting/install/installing-powershell) (for project scripts in `scripts/*.ps1`)
   - Optional but recommended DB client: [SQL Server Management Studio](https://aka.ms/ssmsfullsetup)
   - Optional IDE/editor: VS Code + C# Dev Kit, Rider, or Visual Studio

2. Clone and restore the project

```bash
git clone https://github.com/Michael-Kolpakov/teachio-api.git
cd teachio-api
dotnet restore Teachio.sln
```

3. Configure development environment
   - Set environment to Development.
   - PowerShell (Windows/macOS/Linux):

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

- Bash/Zsh:

```bash
export ASPNETCORE_ENVIRONMENT=Development
```

- Verify `Teachio.WebApi/appsettings.Development.json` contains a valid `ConnectionStrings:DefaultConnection` for your local SQL Server instance.
- Example for local trusted SQL Server connection:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=localhost;Database=teachio-db;TrustServerCertificate=True;MultipleActiveResultSets=True;Trusted_Connection=True;"
}
```

4. Prepare database
   - Install EF Core CLI once:

```bash
dotnet tool install --global dotnet-ef
```

- Apply migrations:

```bash
dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
```

- Note: the app also tries to run migrations and seed data on startup.

5. Run in development mode

```bash
dotnet watch --project Teachio.WebApi run
```

- Swagger UI will be available at:
- `https://localhost:<port>/swagger`
- `http://localhost:<port>/swagger`

6. Basic commands and operations

```bash
# Dev startup automation (Linux/macOS)
./scripts/run/run-dev.sh

# Dev startup automation (Windows)
scripts\run\run-dev.bat

# Production-like startup automation (Linux/macOS)
./scripts/run/run-prod.sh

# Production-like startup automation (Windows)
scripts\run\run-prod.bat

# Build solution
dotnet build Teachio.sln

# Run all tests
dotnet test Teachio.sln

# Verify formatting
dotnet format Teachio.sln --verify-no-changes

# Run full code-quality check script (PowerShell)
pwsh ./scripts/check-code.ps1

# Add and apply a new EF migration
dotnet ef migrations add <MigrationName> --project Teachio.DAL --startup-project Teachio.WebApi
dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
```

---

## Environment Variables

Below is an example list of common variables used in this project:

| Variable                               | Description                        | Example                    |
| -------------------------------------- | ---------------------------------- | -------------------------- |
| `ASPNETCORE_ENVIRONMENT`               | Runtime environment                | `Development`              |
| `ConnectionStrings__DefaultConnection` | Primary database connection string | `Server=...;Database=...;` |
| `Jwt__Issuer`                          | JWT token issuer                   | `teachio-api`              |
| `Jwt__Audience`                        | JWT token audience                 | `teachio-clients`          |
| `Jwt__Key`                             | JWT signing key                    | `your-very-strong-secret`  |
| `Logging__LogLevel__Default`           | Default log level                  | `Information`              |

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

Note: application startup also runs pending migrations and DB seed automatically (except integration-test environment).

---

## API Documentation

For Swagger open:

- `https://localhost:<port>/swagger`
- or `http://localhost:<port>/swagger`

Use this UI to explore endpoints, request/response schemas, and test operations.

---

## Production Deployment Guide

For release engineer and DevOps production rollout instructions, see:

- [docs/production-deployment.md](docs/production-deployment.md)

---

## Backup and Restore Runbook

For backup strategy, automation scripts, integrity checks, and restore procedure, see:

- [docs/backup-and-restore.md](docs/backup-and-restore.md)

---

## DevOps Automation Pack

For deployment/update scripts, CI/CD configs, Docker, Kubernetes/Swarm, and Terraform templates, see:

- [docs/devops-automation.md](docs/devops-automation.md)

---

## Documentation Standards

To keep the project maintainable for new contributors, all public API and behavior changes must be documented in the same pull request.

Required documentation rules:

- Add XML comments (`///`) for every new or changed public class, interface, record, enum, struct, and public method.
- Keep comments concise and useful: purpose, key parameters, return value, side effects, and possible failure cases.
- For endpoint handlers and controllers, document authorization expectations and validation behavior.
- For repository and service methods, document domain assumptions and expected invariants.
- For breaking changes, include migration notes in the PR description and release notes.

Documentation checklist for PR review:

- Public signatures are documented.
- Business rules and edge cases are described where they matter.
- Renamed/removed APIs are reflected in the public API inventory.

---

## Public API Inventory

All current public interfaces (functions, classes, methods, and other public declarations) are documented in:

- [docs/public-api.md](docs/public-api.md)
- [docs/public-api-reference.md](docs/public-api-reference.md)

This inventory is generated from production projects:

- `Teachio.BLL`
- `Teachio.DAL`
- `Teachio.WebApi`

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
