# StyleCop in Teachio

This solution uses StyleCop analyzers at the solution level via `Directory.Build.props`.

## Linter Choice and Rationale

- Selected linter: `StyleCop.Analyzers`.
- Why this linter: it provides strong C# style and readability checks, integrates natively with Roslyn/.NET build, and can be centrally configured for all projects in the solution.
- Why not project-by-project setup: one shared setup in root files keeps behavior consistent across `Teachio.BLL`, `Teachio.DAL`, `Teachio.WebApi`, and test projects.

## What is configured

- `StyleCop.Analyzers` package is referenced once in `Directory.Build.props` and applies to all projects.
- A single root `stylecop.json` is included for all projects as `AdditionalFiles`.
- Root `.editorconfig` controls analyzer severities for all `*.cs` files.
- Build integration is enabled via `EnforceCodeStyleInBuild=true`, `RunAnalyzers=true`, and `RunAnalyzersDuringBuild=true`.

## Build Integration

- StyleCop runs automatically during `dotnet build` for all projects in the solution.
- Analyzer diagnostics are promoted to build errors by `CodeAnalysisTreatWarningsAsErrors=true`.
- General compiler warnings are promoted to errors by `TreatWarningsAsErrors=true`.
- Result: build fails if StyleCop violations are present (except rules explicitly set to `none`).

## Current policy

- StyleCop categories are enabled with `error` severity.
- Analyzer diagnostics are treated as errors in both IDE (severity from `.editorconfig`) and build (`CodeAnalysisTreatWarningsAsErrors=true`).
- A controlled subset of noisy/non-actionable diagnostics remains disabled explicitly.

## Core Rules and Rationale

- Explicit object creation is required (`new ClassName()`), which improves readability in complex diffs and avoids ambiguity.
- File-scoped namespaces are required (`namespace X;`) to reduce nesting noise.
- Braces are required and single-line blocks/statements are disallowed for safer refactoring.
- `using` directives must be declared in the configured order to keep files consistent.
- Private fields must start with `_`, constants must use PascalCase.
- Whitespace hygiene is enforced: no trailing spaces, no multiple consecutive spaces, newline at end of file.
- Some diagnostics are explicitly suppressed by team decision (for example selected CA rules) to reduce noise.

## How to Run Linting

Run full build with analyzers:

```powershell
dotnet build Teachio.sln
```

Run build without NuGet audit blocking (if needed in local environment):

```powershell
dotnet build Teachio.sln -p:NuGetAudit=false
```

Run a full code quality gate (restore, build with analyzers/StyleCop, format check, tests):

```powershell
pwsh -ExecutionPolicy Bypass -File ./scripts/check-code.ps1
```

Useful options:

```powershell
pwsh -ExecutionPolicy Bypass -File ./scripts/check-code.ps1 -Configuration Release
pwsh -ExecutionPolicy Bypass -File ./scripts/check-code.ps1 -SkipFormatCheck
pwsh -ExecutionPolicy Bypass -File ./scripts/check-code.ps1 -SkipTests
```

## Pre-commit Gate

To block commits while there are analyzer/build problems, this repository uses a Git pre-commit hook:

- Hook file: `.githooks/pre-commit`
- Git setting: `core.hooksPath=.githooks`
- Check command: `dotnet build Teachio.sln -p:NuGetAudit=false`

If the build fails, the commit is rejected.

## How to tune rules

Use `.editorconfig` entries:

```ini
dotnet_diagnostic.SA1600.severity = none
```

Severity values: `none`, `silent`, `suggestion`, `warning`, `error`.

Use `stylecop.json` for semantic behavior (ordering, documentation options, layout).
