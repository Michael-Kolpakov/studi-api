#!/usr/bin/env bash
set -euo pipefail

# In-place production update script with basic rollback.

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

RELEASE_ID="${1:-$(date +%Y%m%d-%H%M%S)}"
DEPLOY_ROOT="${DEPLOY_ROOT:-/opt/teachio-api}"
RELEASES_DIR="$DEPLOY_ROOT/releases"
CURRENT_LINK="$DEPLOY_ROOT/current"
PREVIOUS_LINK="$DEPLOY_ROOT/previous"
SERVICE_NAME="${SERVICE_NAME:-teachio-api}"
RUN_MIGRATIONS="${RUN_MIGRATIONS:-1}"

mkdir -p "$RELEASES_DIR"
TARGET_DIR="$RELEASES_DIR/$RELEASE_ID"

PREVIOUS_TARGET=""
if [[ -L "$CURRENT_LINK" ]]; then
  PREVIOUS_TARGET="$(readlink -f "$CURRENT_LINK")"
fi

echo "==> Restore"
dotnet restore Teachio.sln -p:NuGetAudit=false

echo "==> Build"
dotnet build Teachio.sln -c Release --no-restore -p:NuGetAudit=false

echo "==> Publish to $TARGET_DIR"
dotnet publish Teachio.WebApi/Teachio.WebApi.csproj -c Release --no-build -o "$TARGET_DIR"

echo "==> Stop service: $SERVICE_NAME"
sudo systemctl stop "$SERVICE_NAME"

if [[ -n "$PREVIOUS_TARGET" ]]; then
  ln -sfn "$PREVIOUS_TARGET" "$PREVIOUS_LINK"
fi

if [[ "$RUN_MIGRATIONS" == "1" ]]; then
  echo "==> Apply migrations"
  dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
fi

echo "==> Activate release"
ln -sfn "$TARGET_DIR" "$CURRENT_LINK"

echo "==> Start service"
sudo systemctl start "$SERVICE_NAME"

if ! sudo systemctl is-active --quiet "$SERVICE_NAME"; then
  echo "Service failed after update. Rolling back."
  if [[ -L "$PREVIOUS_LINK" ]]; then
    ln -sfn "$(readlink -f "$PREVIOUS_LINK")" "$CURRENT_LINK"
    sudo systemctl start "$SERVICE_NAME"
  fi
  exit 1
fi

sudo systemctl status "$SERVICE_NAME" --no-pager
echo "Update complete. Release: $RELEASE_ID"
