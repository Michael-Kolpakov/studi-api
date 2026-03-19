@echo off
setlocal enabledelayedexpansion

set "ROOT_DIR=%~dp0..\.."
pushd "%ROOT_DIR%"

set "SKIP_RESTORE=0"
set "SKIP_MIGRATE=0"
set "NO_WATCH=0"

:parseArgs
if "%~1"=="" goto argsDone
if /I "%~1"=="--skip-restore" set "SKIP_RESTORE=1" & shift & goto parseArgs
if /I "%~1"=="--skip-migrate" set "SKIP_MIGRATE=1" & shift & goto parseArgs
if /I "%~1"=="--no-watch" set "NO_WATCH=1" & shift & goto parseArgs

echo Unknown argument: %~1
echo Usage: scripts\run\run-dev.bat [--skip-restore] [--skip-migrate] [--no-watch]
exit /b 1

:argsDone
where dotnet >nul 2>nul
if errorlevel 1 (
  echo dotnet SDK is required.
  popd
  exit /b 1
)

set "ASPNETCORE_ENVIRONMENT=Development"
echo ==^> Environment: %ASPNETCORE_ENVIRONMENT%

if "%SKIP_RESTORE%"=="0" (
  echo ==^> Restore
  dotnet restore Teachio.sln -p:NuGetAudit=false
  if errorlevel 1 goto fail
)

if "%SKIP_MIGRATE%"=="0" (
  echo ==^> Apply migrations
  dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
  if errorlevel 1 goto fail
)

if "%NO_WATCH%"=="1" (
  echo ==^> Run API
  dotnet run --project Teachio.WebApi
  if errorlevel 1 goto fail
) else (
  echo ==^> Run API with watch
  dotnet watch --project Teachio.WebApi run
  if errorlevel 1 goto fail
)

popd
exit /b 0

:fail
echo Script failed.
popd
exit /b 1
