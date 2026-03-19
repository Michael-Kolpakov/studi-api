#!/usr/bin/env bash
set -euo pipefail

# Teachio production backup script.
# Supports SQL Server DB backup (full/diff/log), config backup, user data backup, and log backup.

BACKUP_TYPE="full"
for arg in "$@"; do
  case "$arg" in
    --type=full) BACKUP_TYPE="full" ;;
    --type=diff) BACKUP_TYPE="diff" ;;
    --type=log) BACKUP_TYPE="log" ;;
    *)
      echo "Unknown argument: $arg"
      echo "Usage: ./scripts/backup/backup-prod.sh [--type=full|--type=diff|--type=log]"
      exit 1
      ;;
  esac
done

TIMESTAMP="$(date +%Y%m%d-%H%M%S)"

# Paths and retention policy can be overridden by environment variables.
BACKUP_ROOT="${BACKUP_ROOT:-/var/backups/teachio}"
CONFIG_SOURCE_DIR="${CONFIG_SOURCE_DIR:-/opt/teachio-api/current}"
LOG_SOURCE_DIR="${LOG_SOURCE_DIR:-/var/log/teachio-api}"
USER_DATA_DIR="${USER_DATA_DIR:-/move-it-storage}"
SYSTEMD_UNIT_PATH="${SYSTEMD_UNIT_PATH:-/etc/systemd/system/teachio-api.service}"

DB_SERVER="${DB_SERVER:-localhost}"
DB_NAME="${DB_NAME:-teachio-db}"
DB_USER="${DB_USER:-}"
DB_PASSWORD="${DB_PASSWORD:-}"
SQLCMD_PATH="${SQLCMD_PATH:-sqlcmd}"

RETENTION_DAYS_DB="${RETENTION_DAYS_DB:-14}"
RETENTION_DAYS_CONFIG="${RETENTION_DAYS_CONFIG:-30}"
RETENTION_DAYS_USERDATA="${RETENTION_DAYS_USERDATA:-14}"
RETENTION_DAYS_LOGS="${RETENTION_DAYS_LOGS:-7}"

mkdir -p "$BACKUP_ROOT/db/$BACKUP_TYPE" "$BACKUP_ROOT/config" "$BACKUP_ROOT/userdata" "$BACKUP_ROOT/logs"

DB_BACKUP_FILE="$BACKUP_ROOT/db/$BACKUP_TYPE/${DB_NAME}-${BACKUP_TYPE}-${TIMESTAMP}.bak"
CONFIG_ARCHIVE="$BACKUP_ROOT/config/config-${TIMESTAMP}.tar.gz"
USERDATA_ARCHIVE="$BACKUP_ROOT/userdata/userdata-${TIMESTAMP}.tar.gz"
LOGS_ARCHIVE="$BACKUP_ROOT/logs/logs-${TIMESTAMP}.tar.gz"

sql_auth_args=("-S" "$DB_SERVER")
if [[ -n "$DB_USER" && -n "$DB_PASSWORD" ]]; then
  sql_auth_args+=("-U" "$DB_USER" "-P" "$DB_PASSWORD")
else
  sql_auth_args+=("-E")
fi

case "$BACKUP_TYPE" in
  full)
    SQL_QUERY="BACKUP DATABASE [$DB_NAME] TO DISK = N'$DB_BACKUP_FILE' WITH INIT, CHECKSUM, COMPRESSION, STATS = 10;"
    ;;
  diff)
    SQL_QUERY="BACKUP DATABASE [$DB_NAME] TO DISK = N'$DB_BACKUP_FILE' WITH DIFFERENTIAL, INIT, CHECKSUM, COMPRESSION, STATS = 10;"
    ;;
  log)
    SQL_QUERY="BACKUP LOG [$DB_NAME] TO DISK = N'$DB_BACKUP_FILE' WITH INIT, CHECKSUM, COMPRESSION, STATS = 10;"
    ;;
  *)
    echo "Unsupported backup type: $BACKUP_TYPE"
    exit 1
    ;;
esac

echo "==> Backing up SQL Server database: $DB_NAME ($BACKUP_TYPE)"
"$SQLCMD_PATH" "${sql_auth_args[@]}" -Q "$SQL_QUERY"

echo "==> Verifying SQL backup integrity"
"$SQLCMD_PATH" "${sql_auth_args[@]}" -Q "RESTORE VERIFYONLY FROM DISK = N'$DB_BACKUP_FILE';"

echo "==> Backing up configuration files"
if [[ -d "$CONFIG_SOURCE_DIR" ]]; then
  tmp_cfg_dir="$(mktemp -d)"
  if compgen -G "$CONFIG_SOURCE_DIR/appsettings*.json" > /dev/null; then
    cp "$CONFIG_SOURCE_DIR"/appsettings*.json "$tmp_cfg_dir"/
  fi

  if [[ -f "$SYSTEMD_UNIT_PATH" ]]; then
    cp "$SYSTEMD_UNIT_PATH" "$tmp_cfg_dir"/
  fi

  tar -czf "$CONFIG_ARCHIVE" -C "$tmp_cfg_dir" .
  rm -rf "$tmp_cfg_dir"
else
  echo "WARN: config source directory not found: $CONFIG_SOURCE_DIR"
fi

echo "==> Backing up user data"
if [[ -d "$USER_DATA_DIR" ]]; then
  tar -czf "$USERDATA_ARCHIVE" -C "$USER_DATA_DIR" .
else
  echo "WARN: user data directory not found: $USER_DATA_DIR"
fi

echo "==> Backing up service logs"
if [[ -d "$LOG_SOURCE_DIR" ]]; then
  tar -czf "$LOGS_ARCHIVE" -C "$LOG_SOURCE_DIR" .
else
  echo "WARN: log source directory not found: $LOG_SOURCE_DIR"
fi

echo "==> Applying retention policy"
find "$BACKUP_ROOT/db" -type f -name "*.bak" -mtime +"$RETENTION_DAYS_DB" -delete || true
find "$BACKUP_ROOT/config" -type f -name "*.tar.gz" -mtime +"$RETENTION_DAYS_CONFIG" -delete || true
find "$BACKUP_ROOT/userdata" -type f -name "*.tar.gz" -mtime +"$RETENTION_DAYS_USERDATA" -delete || true
find "$BACKUP_ROOT/logs" -type f -name "*.tar.gz" -mtime +"$RETENTION_DAYS_LOGS" -delete || true

echo "Backup completed successfully."
echo "DB backup: $DB_BACKUP_FILE"
if [[ -f "$CONFIG_ARCHIVE" ]]; then echo "Config archive: $CONFIG_ARCHIVE"; fi
if [[ -f "$USERDATA_ARCHIVE" ]]; then echo "User data archive: $USERDATA_ARCHIVE"; fi
if [[ -f "$LOGS_ARCHIVE" ]]; then echo "Logs archive: $LOGS_ARCHIVE"; fi
