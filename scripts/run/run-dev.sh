#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

SKIP_RESTORE=0
SKIP_MIGRATE=0
NO_WATCH=0

for arg in "$@"; do
  case "$arg" in
    --skip-restore) SKIP_RESTORE=1 ;;
    --skip-migrate) SKIP_MIGRATE=1 ;;
    --no-watch) NO_WATCH=1 ;;
    *)
      echo "Unknown argument: $arg"
      echo "Usage: ./scripts/run/run-dev.sh [--skip-restore] [--skip-migrate] [--no-watch]"
      exit 1
      ;;
  esac
done

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet SDK is required."
  exit 1
fi

export ASPNETCORE_ENVIRONMENT=Development

echo "==> Environment: $ASPNETCORE_ENVIRONMENT"

if [[ "$SKIP_RESTORE" -eq 0 ]]; then
  echo "==> Restore"
  dotnet restore Teachio.sln -p:NuGetAudit=false
fi

if [[ "$SKIP_MIGRATE" -eq 0 ]]; then
  echo "==> Apply migrations"
  dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
fi

if [[ "$NO_WATCH" -eq 1 ]]; then
  echo "==> Run API"
  dotnet run --project Teachio.WebApi
else
  echo "==> Run API with watch"
  dotnet watch --project Teachio.WebApi run
fi
