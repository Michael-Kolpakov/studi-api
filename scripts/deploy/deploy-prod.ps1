param(
    [string]$ReleaseId = (Get-Date -Format "yyyyMMdd-HHmmss"),
    [string]$DeployRoot = "/opt/teachio-api",
    [string]$ServiceName = "teachio-api",
    [switch]$SkipMigrations
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $PSCommandPath
$repoRoot = Resolve-Path (Join-Path $scriptRoot "../..")
Set-Location $repoRoot

$releasesDir = Join-Path $DeployRoot "releases"
$currentLink = Join-Path $DeployRoot "current"
$targetDir = Join-Path $releasesDir $ReleaseId

New-Item -ItemType Directory -Path $releasesDir -Force | Out-Null

Write-Host "==> Restore" -ForegroundColor Cyan
dotnet restore Teachio.sln -p:NuGetAudit=false

Write-Host "==> Build" -ForegroundColor Cyan
dotnet build Teachio.sln -c Release --no-restore -p:NuGetAudit=false

Write-Host "==> Publish to $targetDir" -ForegroundColor Cyan
dotnet publish Teachio.WebApi/Teachio.WebApi.csproj -c Release --no-build -o $targetDir

if (-not $SkipMigrations.IsPresent) {
    Write-Host "==> Apply migrations" -ForegroundColor Cyan
    dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
}

Write-Host "==> Switch current symlink" -ForegroundColor Cyan
if (Get-Command ln -ErrorAction SilentlyContinue) {
    & ln -sfn $targetDir $currentLink
}
else {
    if (Test-Path $currentLink) {
        Remove-Item $currentLink -Force
    }
    New-Item -ItemType SymbolicLink -Path $currentLink -Target $targetDir | Out-Null
}

Write-Host "==> Restart service: $ServiceName" -ForegroundColor Cyan
if ($IsLinux -or $IsMacOS) {
    sudo systemctl daemon-reload | Out-Null
    sudo systemctl restart $ServiceName
    sudo systemctl status $ServiceName --no-pager
}
else {
    Restart-Service -Name $ServiceName -ErrorAction Stop
    Get-Service -Name $ServiceName
}

Write-Host "Deployment complete. Release: $ReleaseId" -ForegroundColor Green
