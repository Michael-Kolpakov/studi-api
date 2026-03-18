param(
    [switch]$SkipEnglishCheck,
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Write-Output "Generating public API inventory..."
& "$PSScriptRoot/generate-public-api-inventory.ps1"

Write-Output "Generating public API reference..."
& "$PSScriptRoot/generate-public-api-reference.ps1"

if (-not $SkipEnglishCheck) {
    Write-Output "Checking docs for non-English (Cyrillic) characters..."

    $matches = Select-String -Path "docs/*.md", "Teachio.BLL/**/*.cs", "Teachio.DAL/**/*.cs", "Teachio.WebApi/**/*.cs" -Pattern "[А-Яа-яЁё]" -AllMatches -ErrorAction SilentlyContinue

    if ($matches) {
        Write-Error "Non-English characters found in documentation."
        $matches | ForEach-Object { Write-Output ("{0}:{1}: {2}" -f $_.Path, $_.LineNumber, $_.Line.Trim()) }
        exit 1
    }
}

if (-not $SkipBuild) {
    Write-Output "Running build validation..."
    dotnet build Teachio.sln -p:NuGetAudit=false
}

Write-Output "Documentation generation complete."
