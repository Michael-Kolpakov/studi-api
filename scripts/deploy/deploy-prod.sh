#!/usr/bin/env bash
set -euo pipefail

# Fresh production deployment script.
# Publishes app to a release folder and switches current symlink.

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

RELEASE_ID="${1:-$(date +%Y%m%d-%H%M%S)}"
DEPLOY_ROOT="${DEPLOY_ROOT:-/opt/teachio-api}"
RELEASES_DIR="$DEPLOY_ROOT/releases"
CURRENT_LINK="$DEPLOY_ROOT/current"
SERVICE_NAME="${SERVICE_NAME:-teachio-api}"
RUN_MIGRATIONS="${RUN_MIGRATIONS:-1}"

mkdir -p "$RELEASES_DIR"
TARGET_DIR="$RELEASES_DIR/$RELEASE_ID"

echo "==> Restore"
dotnet restore Teachio.sln -p:NuGetAudit=false

echo "==> Build"
dotnet build Teachio.sln -c Release --no-restore -p:NuGetAudit=false

echo "==> Publish to $TARGET_DIR"
dotnet publish Teachio.WebApi/Teachio.WebApi.csproj -c Release --no-build -o "$TARGET_DIR"

if [[ "$RUN_MIGRATIONS" == "1" ]]; then
  echo "==> Apply migrations"
  dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
fi

echo "==> Switch current symlink"
ln -sfn "$TARGET_DIR" "$CURRENT_LINK"

echo "==> Restart service: $SERVICE_NAME"
sudo systemctl daemon-reload || true
sudo systemctl restart "$SERVICE_NAME"
sudo systemctl status "$SERVICE_NAME" --no-pager

echo "Deployment complete. Release: $RELEASE_ID"
