# Documentation Generation Guide

This guide explains how to regenerate project documentation consistently.

## Scope

The documentation process covers:

- XML comments for public API in source code (`public` classes, interfaces, records, enums, structs, delegates, methods, and constructors)
- Public API inventory: [docs/public-api.md](docs/public-api.md)
- Public API reference: [docs/public-api-reference.md](docs/public-api-reference.md)

## Prerequisites

- .NET SDK installed
- PowerShell available
- Run commands from repository root

## Script-Based Workflow

All generation commands are available in:

- [scripts/docs/generate-public-api-inventory.ps1](scripts/docs/generate-public-api-inventory.ps1)
- [scripts/docs/generate-public-api-reference.ps1](scripts/docs/generate-public-api-reference.ps1)
- [scripts/docs/generate-docs.ps1](scripts/docs/generate-docs.ps1)

Recommended one-command flow:

```powershell
pwsh -ExecutionPolicy Bypass -File ./scripts/docs/generate-docs.ps1
```

Optional flags:

```powershell
pwsh -ExecutionPolicy Bypass -File ./scripts/docs/generate-docs.ps1 -SkipEnglishCheck
pwsh -ExecutionPolicy Bypass -File ./scripts/docs/generate-docs.ps1 -SkipBuild
pwsh -ExecutionPolicy Bypass -File ./scripts/docs/generate-docs.ps1 -SkipEnglishCheck -SkipBuild
```

## 1) Update XML Documentation in Code

Add or update `///` XML comments for all public API members in changed files.

Required tags (depending on member):

- `<summary>` for all public members
- `<param>` for each method/constructor parameter
- `<typeparam>` for generic type parameters
- `<returns>` only for non-`void` methods

Style notes:

- Use English text only
- For properties, follow StyleCop wording:
  - `Gets ...`
  - `Sets ...`
  - `Gets or sets ...`

## 2) Regenerate Public API Inventory

Run:

```powershell
pwsh -ExecutionPolicy Bypass -File ./scripts/docs/generate-public-api-inventory.ps1
```

## 3) Regenerate Public API Reference

Run:

```powershell
pwsh -ExecutionPolicy Bypass -File ./scripts/docs/generate-public-api-reference.ps1
```

## 4) Verify English-Only Documentation

Run checks:

```powershell
Select-String -Path docs\*.md,Teachio.*\**\*.cs -Pattern '[А-Яа-яЁё]' -AllMatches
```

Expected result: no matches.

## 5) Validate Build

Run:

```powershell
dotnet build Teachio.sln -p:NuGetAudit=false
```

Expected result: build succeeds with no documentation-related errors.

## 6) Commit Policy

In one pull request, include:

- source code changes
- XML comments updates
- updated [docs/public-api.md](docs/public-api.md)
- updated [docs/public-api-reference.md](docs/public-api-reference.md)
