param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$SkipRestore,
    [switch]$SkipFormatCheck,
    [switch]$SkipTests
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-Step {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,
        [Parameter(Mandatory = $true)]
        [scriptblock]$Action
    )

    Write-Host "==> $Name" -ForegroundColor Cyan
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "Step failed: $Name"
    }
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
Push-Location $repoRoot

try {
    if (-not $SkipRestore) {
        Invoke-Step -Name "Restore" -Action {
            dotnet restore Teachio.sln -p:NuGetAudit=false --nologo
        }
    }

    Invoke-Step -Name "Build + StyleCop analyzers" -Action {
        dotnet build Teachio.sln -c $Configuration -p:NuGetAudit=false -p:RunAnalyzers=true -p:RunAnalyzersDuringBuild=true --nologo -v minimal
    }

    if (-not $SkipFormatCheck) {
        Invoke-Step -Name "Formatting check (no changes allowed)" -Action {
            dotnet format Teachio.sln --verify-no-changes --severity error --no-restore --verbosity minimal
        }
    }

    if (-not $SkipTests) {
        Invoke-Step -Name "Tests" -Action {
            dotnet test Teachio.sln -c $Configuration -p:NuGetAudit=false --no-build --nologo -v minimal
        }
    }

    Write-Host "All code quality checks passed." -ForegroundColor Green
}
finally {
    Pop-Location
}
