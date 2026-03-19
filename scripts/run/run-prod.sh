#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

SKIP_RESTORE=0
SKIP_BUILD=0
SKIP_MIGRATE=0

for arg in "$@"; do
  case "$arg" in
    --skip-restore) SKIP_RESTORE=1 ;;
    --skip-build) SKIP_BUILD=1 ;;
    --skip-migrate) SKIP_MIGRATE=1 ;;
    *)
      echo "Unknown argument: $arg"
      echo "Usage: ./scripts/run/run-prod.sh [--skip-restore] [--skip-build] [--skip-migrate]"
      exit 1
      ;;
  esac
done

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet runtime/SDK is required."
  exit 1
fi

export ASPNETCORE_ENVIRONMENT=Production

echo "==> Environment: $ASPNETCORE_ENVIRONMENT"

if [[ "$SKIP_RESTORE" -eq 0 ]]; then
  echo "==> Restore"
  dotnet restore Teachio.sln -p:NuGetAudit=false
fi

if [[ "$SKIP_BUILD" -eq 0 ]]; then
  echo "==> Build Release"
  dotnet build Teachio.sln -c Release --no-restore -p:NuGetAudit=false
fi

if [[ "$SKIP_MIGRATE" -eq 0 ]]; then
  echo "==> Apply migrations"
  dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
fi

echo "==> Run API (Release)"
dotnet run --project Teachio.WebApi -c Release --no-build
