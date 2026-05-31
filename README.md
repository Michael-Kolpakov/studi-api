# Studi API

A robust, scalable, and maintainable **ASP.NET Code Web API (C#)** for the LMS Studi platform.  
This repository contains the backend service responsible for business logic, data access, and external integrations required by Studi clients.

> **Status:** Active development

---

## Overview

**studi-api** is the backend API layer for the Studi ecosystem.
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

# Studi — Backend (Studi.WebApi)

## Overview

Studi is the backend service for an online learning platform. This repository contains the ASP.NET Core Web API (`Studi.WebApi`), the business logic layer (`Studi.BLL`), the data access layer (`Studi.DAL`), and unit/integration tests.

## Purpose

This README provides step-by-step, reproducible instructions for a developer starting from a clean Windows machine to get the service running locally and to run tests.

## Table of Contents

- [Overview](#overview)
- [Purpose](#purpose)
- [Requirements](#requirements-clean-windows)
- [Quick start](#quick-start-recommended-using-docker-for-sql-server)
- [Configuration](#configure-local-settings)
- [Database and migrations](#apply-ef-core-migrations-creates-database-schema)
- [Running the API](#run-the-api)
- [Tests](#tests)
- [Configuration summary (critical keys)](#configuration-summary-critical-keys)
- [Helpful commands](#helpful-commands)
- [Troubleshooting](#troubleshooting)
- [Security and secrets](#security-and-secrets)
- [Contributing](#contributing)
- [License](#license)
- [Where to look in the code](#where-to-look-in-the-code)

## Requirements (clean Windows)

- Windows 10/11
- .NET SDK 9.x — install from https://dotnet.microsoft.com/download/dotnet/9.0
- Git — https://git-scm.com/
- MS SQL Server (Developer/Express) or Docker (recommended) with SQL Server image `mcr.microsoft.com/mssql/server:2022-latest`
- `dotnet-ef` CLI tool for Entity Framework Core migrations: `dotnet tool install --global dotnet-ef`
- FFmpeg (`ffprobe`) — either in PATH or specify full path in configuration
- (Optional) Visual Studio 2022/2023 or Visual Studio Code

## Quick start (recommended using Docker for SQL Server)

1. Clone repository

```bash
git clone https://github.com/Michael-Kolpakov/studi-api.git
cd studi-api
```

2. Verify .NET SDK and install `dotnet-ef` if necessary

```powershell
dotnet --version
dotnet tool install --global dotnet-ef
```

3. Start SQL Server in Docker (recommended)

```powershell
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=DevPass123!" -p 1433:1433 --name studi-mssql -d mcr.microsoft.com/mssql/server:2022-latest
```

4. Configure local settings

- Open `Studi.WebApi/appsettings.Local.json` and set required values:
  - `ConnectionStrings:DefaultConnection` — connection string to your SQL Server (example provided uses `localhost` and password `DevPass123!`).
  - `Jwt:SigningKey` — set a secure signing key for JWT tokens.
  - `Smtp` — SMTP host/port/credentials if the application will send email.
  - `GoogleDriveStorage` — fill only if Google Drive integration is needed.
  - `Ffprobe:ExecutablePath` — set either `ffprobe` (if in PATH) or full path to `ffprobe.exe`.

5. Restore packages and build

```powershell
dotnet restore
dotnet build -c Debug
```

6. Apply EF Core migrations (creates database schema)

```powershell
dotnet ef database update --project "Studi.DAL" --startup-project "Studi.WebApi"
```

7. Run the API

```powershell
cd Studi.WebApi
dotnet run
```

The application will start and print listening URLs. By default, Swagger UI is available at `/swagger` (e.g. `https://localhost:7xxx/swagger`).

## Running in Visual Studio

Open `Studi.sln` in Visual Studio 2022/2023 (with .NET 9 SDK installed) and set `Studi.WebApi` as the startup project, then run with the debugger.

## Tests

Run all tests in the solution:

```powershell
dotnet test
```

Run a single test project (example):

```powershell
dotnet test Studi.XUnitTests/Studi.XUnitTests.csproj
```

## Configuration summary (critical keys)

- `ConnectionStrings:DefaultConnection` — database connection string used by EF Core.
- `Jwt:SigningKey` — secret used to sign JWT tokens; keep it secure.
- `Smtp` — SMTP configuration for sending emails.
- `GoogleDriveStorage` — OAuth/service account settings for Google Drive integration.
- `Ffprobe:ExecutablePath` — path or binary name for `ffprobe` used by video processing.

All example values and defaults are present in `Studi.WebApi/appsettings.Local.json`.

## Helpful commands

- Restore and build: `dotnet restore && dotnet build`
- Apply migrations: `dotnet ef database update --project "Studi.DAL" --startup-project "Studi.WebApi"`
- Create migration: `dotnet ef migrations add MyMigration --project "Studi.DAL" --startup-project "Studi.WebApi"`
- Run SQL Server in Docker: use the `docker run` command above.
- Verify `ffprobe`: `ffprobe --version`

## StyleCop (Code Style and Analyzers)

- Configuration files:
  - Root StyleCop settings: `stylecop.json` (repository root).
  - Shared analyzer package and build flags: `Directory.Build.props` (references `StyleCop.Analyzers`, enables `RunAnalyzers` and treats analyzer diagnostics as errors).
  - Analyzer severities and overrides: `.editorconfig` (repository root).
- Where to find full guidance: `STYLECOP.md` (detailed team policy and commands).
- How it is enforced:
  - StyleCop runs as Roslyn analyzers during `dotnet build` for all projects in the solution.
  - Build-time flags (`EnforceCodeStyleInBuild`, `RunAnalyzersDuringBuild`, `CodeAnalysisTreatWarningsAsErrors`) promote analyzer diagnostics to errors — CI and local builds fail on violations.
  - A `check-code.ps1` script (in `scripts/`) runs the full code quality gate (restore, build with analyzers, format check, tests).
- IDE integration and developer workflow:
  - Visual Studio and VS Code (with C# extension) show StyleCop diagnostics inline — fix warnings/errors in the editor.
  - To run analyzers locally: `dotnet build Studi.sln` (or use the `check-code.ps1` script for the full gate).
  - To temporarily bypass analyzer failures for quick experiments, you may build with `-p:RunAnalyzers=false` (not recommended for commits/PRs).
- Pre-commit and CI:
  - The repository provides a Git hook (`.githooks/pre-commit`) and sets `core.hooksPath=.githooks` to run the build and block commits with analyzer errors.
  - CI pipelines should run `dotnet build` (with analyzers) and `dotnet test` as part of the quality gate.
- Tuning rules:
  - Change severity per rule using `.editorconfig` (for example: `dotnet_diagnostic.SA1600.severity = none`).
  - Change behavioral rules (ordering, layout, documentation options) in `stylecop.json`.
  - Use `Directory.Build.props` to update analyzer package version or global analyzer settings.

This section summarizes StyleCop usage in this repository; see `STYLECOP.md` for the full policy, examples, and script options.

## Troubleshooting

- "Cannot connect to SQL Server": Ensure the SQL Server instance or Docker container is running, port 1433 is reachable, and `ConnectionStrings:DefaultConnection` is correct.
- "dotnet-ef not found": Install the tool with `dotnet tool install --global dotnet-ef` and restart your terminal.
- "ffprobe not found": Install FFmpeg and ensure `ffprobe` is in PATH or set `Ffprobe:ExecutablePath` to the executable path.
- Migration errors: confirm `--startup-project` is `Studi.WebApi` and `--project` is `Studi.DAL` when running EF commands.

## Security and secrets

- Do not commit secrets. Use `appsettings.Local.json` for local development, environment variables, or `dotnet user-secrets` for per-developer secrets.
- In production, use a secrets manager (Azure Key Vault, AWS Secrets Manager, etc.).

## Contributing

Contributions are welcome. Please fork the repository, create a feature branch, and open a pull request. Follow conventional commits and keep PRs focused.

## License

This repository is licensed under the MIT License. See the `LICENSE` file in the repository root.

## Where to look in the code

- API and startup: `Studi.WebApi`
- Database context and migrations: `Studi.DAL/Persistence`
- Business logic: `Studi.BLL`
