@echo off
setlocal EnableDelayedExpansion

REM Teachio production backup script.
REM Supports SQL Server DB backup (full/diff/log), config backup, user data backup, and log backup.

set "BACKUP_TYPE=full"
:parseArgs
if "%~1"=="" goto argsDone
if /I "%~1"=="--type=full" set "BACKUP_TYPE=full" & shift & goto parseArgs
if /I "%~1"=="--type=diff" set "BACKUP_TYPE=diff" & shift & goto parseArgs
if /I "%~1"=="--type=log" set "BACKUP_TYPE=log" & shift & goto parseArgs

echo Unknown argument: %~1
echo Usage: scripts\backup\backup-prod.bat [--type=full^|--type=diff^|--type=log]
exit /b 1

:argsDone
if "%BACKUP_ROOT%"=="" set "BACKUP_ROOT=C:\backups\teachio"
if "%CONFIG_SOURCE_DIR%"=="" set "CONFIG_SOURCE_DIR=C:\opt\teachio-api\current"
if "%LOG_SOURCE_DIR%"=="" set "LOG_SOURCE_DIR=C:\var\log\teachio-api"
if "%USER_DATA_DIR%"=="" set "USER_DATA_DIR=C:\move-it-storage"
if "%SERVICE_CONFIG_PATH%"=="" set "SERVICE_CONFIG_PATH=C:\services\teachio-api-service.txt"

if "%DB_SERVER%"=="" set "DB_SERVER=localhost"
if "%DB_NAME%"=="" set "DB_NAME=teachio-db"
if "%SQLCMD_PATH%"=="" set "SQLCMD_PATH=sqlcmd"

if "%RETENTION_DAYS_DB%"=="" set "RETENTION_DAYS_DB=14"
if "%RETENTION_DAYS_CONFIG%"=="" set "RETENTION_DAYS_CONFIG=30"
if "%RETENTION_DAYS_USERDATA%"=="" set "RETENTION_DAYS_USERDATA=14"
if "%RETENTION_DAYS_LOGS%"=="" set "RETENTION_DAYS_LOGS=7"

for /f %%I in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"') do set "TIMESTAMP=%%I"

set "DB_DIR=%BACKUP_ROOT%\db\%BACKUP_TYPE%"
set "CFG_DIR=%BACKUP_ROOT%\config"
set "USR_DIR=%BACKUP_ROOT%\userdata"
set "LOG_DIR=%BACKUP_ROOT%\logs"

if not exist "%DB_DIR%" mkdir "%DB_DIR%"
if not exist "%CFG_DIR%" mkdir "%CFG_DIR%"
if not exist "%USR_DIR%" mkdir "%USR_DIR%"
if not exist "%LOG_DIR%" mkdir "%LOG_DIR%"

set "DB_BACKUP_FILE=%DB_DIR%\%DB_NAME%-%BACKUP_TYPE%-%TIMESTAMP%.bak"
set "CONFIG_ARCHIVE=%CFG_DIR%\config-%TIMESTAMP%.zip"
set "USER_ARCHIVE=%USR_DIR%\userdata-%TIMESTAMP%.zip"
set "LOG_ARCHIVE=%LOG_DIR%\logs-%TIMESTAMP%.zip"

set "AUTH_ARGS=-S %DB_SERVER%"
if defined DB_USER (
  if defined DB_PASSWORD (
    set "AUTH_ARGS=!AUTH_ARGS! -U %DB_USER% -P %DB_PASSWORD%"
  ) else (
    set "AUTH_ARGS=!AUTH_ARGS! -E"
  )
) else (
  set "AUTH_ARGS=!AUTH_ARGS! -E"
)

if /I "%BACKUP_TYPE%"=="full" (
  set "SQL_QUERY=BACKUP DATABASE [%DB_NAME%] TO DISK = N'%DB_BACKUP_FILE%' WITH INIT, CHECKSUM, COMPRESSION, STATS = 10;"
) else if /I "%BACKUP_TYPE%"=="diff" (
  set "SQL_QUERY=BACKUP DATABASE [%DB_NAME%] TO DISK = N'%DB_BACKUP_FILE%' WITH DIFFERENTIAL, INIT, CHECKSUM, COMPRESSION, STATS = 10;"
) else if /I "%BACKUP_TYPE%"=="log" (
  set "SQL_QUERY=BACKUP LOG [%DB_NAME%] TO DISK = N'%DB_BACKUP_FILE%' WITH INIT, CHECKSUM, COMPRESSION, STATS = 10;"
) else (
  echo Unsupported backup type: %BACKUP_TYPE%
  exit /b 1
)

echo ==^> Backing up SQL Server database: %DB_NAME% (%BACKUP_TYPE%)
%SQLCMD_PATH% %AUTH_ARGS% -Q "%SQL_QUERY%"
if errorlevel 1 goto fail

echo ==^> Verifying SQL backup integrity
%SQLCMD_PATH% %AUTH_ARGS% -Q "RESTORE VERIFYONLY FROM DISK = N'%DB_BACKUP_FILE%';"
if errorlevel 1 goto fail

echo ==^> Backing up configuration files
if exist "%CONFIG_SOURCE_DIR%" (
  powershell -NoProfile -Command "$ErrorActionPreference='Stop'; $src='%CONFIG_SOURCE_DIR%'; $dest='%CONFIG_ARCHIVE%'; $tmp=Join-Path $env:TEMP ('teachio-cfg-' + [Guid]::NewGuid().ToString()); New-Item -ItemType Directory -Path $tmp | Out-Null; Get-ChildItem -Path $src -Filter 'appsettings*.json' -ErrorAction SilentlyContinue | Copy-Item -Destination $tmp -Force; if (Test-Path '%SERVICE_CONFIG_PATH%') { Copy-Item '%SERVICE_CONFIG_PATH%' -Destination $tmp -Force }; Compress-Archive -Path (Join-Path $tmp '*') -DestinationPath $dest -Force; Remove-Item -Recurse -Force $tmp"
  if errorlevel 1 goto fail
) else (
  echo WARN: config source directory not found: %CONFIG_SOURCE_DIR%
)

echo ==^> Backing up user data
if exist "%USER_DATA_DIR%" (
  powershell -NoProfile -Command "$ErrorActionPreference='Stop'; Compress-Archive -Path '%USER_DATA_DIR%\*' -DestinationPath '%USER_ARCHIVE%' -Force"
  if errorlevel 1 goto fail
) else (
  echo WARN: user data directory not found: %USER_DATA_DIR%
)

echo ==^> Backing up service logs
if exist "%LOG_SOURCE_DIR%" (
  powershell -NoProfile -Command "$ErrorActionPreference='Stop'; Compress-Archive -Path '%LOG_SOURCE_DIR%\*' -DestinationPath '%LOG_ARCHIVE%' -Force"
  if errorlevel 1 goto fail
) else (
  echo WARN: log source directory not found: %LOG_SOURCE_DIR%
)

echo ==^> Applying retention policy
powershell -NoProfile -Command "$ErrorActionPreference='Continue'; $root='%BACKUP_ROOT%'; Get-ChildItem -Path (Join-Path $root 'db') -Filter '*.bak' -Recurse | Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-%RETENTION_DAYS_DB%) } | Remove-Item -Force; Get-ChildItem -Path (Join-Path $root 'config') -Filter '*.zip' -Recurse | Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-%RETENTION_DAYS_CONFIG%) } | Remove-Item -Force; Get-ChildItem -Path (Join-Path $root 'userdata') -Filter '*.zip' -Recurse | Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-%RETENTION_DAYS_USERDATA%) } | Remove-Item -Force; Get-ChildItem -Path (Join-Path $root 'logs') -Filter '*.zip' -Recurse | Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-%RETENTION_DAYS_LOGS%) } | Remove-Item -Force"

echo Backup completed successfully.
echo DB backup: %DB_BACKUP_FILE%
if exist "%CONFIG_ARCHIVE%" echo Config archive: %CONFIG_ARCHIVE%
if exist "%USER_ARCHIVE%" echo User data archive: %USER_ARCHIVE%
if exist "%LOG_ARCHIVE%" echo Logs archive: %LOG_ARCHIVE%
exit /b 0

:fail
echo Backup script failed.
exit /b 1
