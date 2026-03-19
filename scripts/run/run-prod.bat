@echo off
setlocal enabledelayedexpansion

set "ROOT_DIR=%~dp0..\.."
pushd "%ROOT_DIR%"

set "SKIP_RESTORE=0"
set "SKIP_BUILD=0"
set "SKIP_MIGRATE=0"

:parseArgs
if "%~1"=="" goto argsDone
if /I "%~1"=="--skip-restore" set "SKIP_RESTORE=1" & shift & goto parseArgs
if /I "%~1"=="--skip-build" set "SKIP_BUILD=1" & shift & goto parseArgs
if /I "%~1"=="--skip-migrate" set "SKIP_MIGRATE=1" & shift & goto parseArgs

echo Unknown argument: %~1
echo Usage: scripts\run\run-prod.bat [--skip-restore] [--skip-build] [--skip-migrate]
exit /b 1

:argsDone
where dotnet >nul 2>nul
if errorlevel 1 (
  echo dotnet runtime/SDK is required.
  popd
  exit /b 1
)

set "ASPNETCORE_ENVIRONMENT=Production"
echo ==^> Environment: %ASPNETCORE_ENVIRONMENT%

if "%SKIP_RESTORE%"=="0" (
  echo ==^> Restore
  dotnet restore Teachio.sln -p:NuGetAudit=false
  if errorlevel 1 goto fail
)

if "%SKIP_BUILD%"=="0" (
  echo ==^> Build Release
  dotnet build Teachio.sln -c Release --no-restore -p:NuGetAudit=false
  if errorlevel 1 goto fail
)

if "%SKIP_MIGRATE%"=="0" (
  echo ==^> Apply migrations
  dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
  if errorlevel 1 goto fail
)

echo ==^> Run API (Release)
dotnet run --project Teachio.WebApi -c Release --no-build
if errorlevel 1 goto fail

popd
exit /b 0

:fail
echo Script failed.
popd
exit /b 1
