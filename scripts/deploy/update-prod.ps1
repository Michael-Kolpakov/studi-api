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
$previousLink = Join-Path $DeployRoot "previous"
$targetDir = Join-Path $releasesDir $ReleaseId

New-Item -ItemType Directory -Path $releasesDir -Force | Out-Null

$previousTarget = $null
if (Test-Path $currentLink) {
    $item = Get-Item $currentLink -ErrorAction SilentlyContinue
    if ($null -ne $item -and $item.LinkType) {
        $previousTarget = $item.Target
    }
}

Write-Host "==> Restore" -ForegroundColor Cyan
dotnet restore Teachio.sln -p:NuGetAudit=false

Write-Host "==> Build" -ForegroundColor Cyan
dotnet build Teachio.sln -c Release --no-restore -p:NuGetAudit=false

Write-Host "==> Publish to $targetDir" -ForegroundColor Cyan
dotnet publish Teachio.WebApi/Teachio.WebApi.csproj -c Release --no-build -o $targetDir

Write-Host "==> Stop service: $ServiceName" -ForegroundColor Cyan
if ($IsLinux -or $IsMacOS) {
    sudo systemctl stop $ServiceName
}
else {
    Stop-Service -Name $ServiceName -ErrorAction Stop
}

if ($previousTarget) {
    if (Test-Path $previousLink) {
        Remove-Item $previousLink -Force
    }
    New-Item -ItemType SymbolicLink -Path $previousLink -Target $previousTarget | Out-Null
}

if (-not $SkipMigrations.IsPresent) {
    Write-Host "==> Apply migrations" -ForegroundColor Cyan
    dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
}

Write-Host "==> Activate release" -ForegroundColor Cyan
if (Test-Path $currentLink) {
    Remove-Item $currentLink -Force
}
New-Item -ItemType SymbolicLink -Path $currentLink -Target $targetDir | Out-Null

Write-Host "==> Start service" -ForegroundColor Cyan
if ($IsLinux -or $IsMacOS) {
    sudo systemctl start $ServiceName
    $active = (sudo systemctl is-active $ServiceName).Trim()
    if ($active -ne "active") {
        Write-Host "Service failed after update. Rolling back." -ForegroundColor Yellow
        if (Test-Path $previousLink) {
            $rollbackTarget = (Get-Item $previousLink).Target
            Remove-Item $currentLink -Force
            New-Item -ItemType SymbolicLink -Path $currentLink -Target $rollbackTarget | Out-Null
            sudo systemctl start $ServiceName
        }
        throw "Update failed"
    }

    sudo systemctl status $ServiceName --no-pager
}
else {
    Start-Service -Name $ServiceName
    $service = Get-Service -Name $ServiceName
    if ($service.Status -ne 'Running') {
        throw "Update failed. Service did not start."
    }
    $service
}

Write-Host "Update complete. Release: $ReleaseId" -ForegroundColor Green
