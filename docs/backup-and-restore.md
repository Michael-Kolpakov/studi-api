# Backup and Restore Runbook (Release Engineer / DevOps)

This document defines backup strategy, execution procedure, integrity validation, automation, and restore process for Teachio API production environment.

## Scope

- Application: Teachio API (.NET 9, ASP.NET Core).
- Database: Microsoft SQL Server (`teachio-db` by default).
- Runtime assets: app configuration, user data directory, and service logs.

## 1. Backup Strategy

### 1.1 Types of backups

1. Full backup

- Complete copy of database and required runtime assets.
- Base point for disaster recovery.

2. Differential backup

- Changes since the last full backup.
- Faster than full backup, useful for daily restore points.

3. Incremental backup

- For SQL Server in this runbook, incremental behavior is covered by transaction log backups.
- Captures changes since the previous log backup.

Recommended SQL Server policy:

- Weekly full backup.
- Daily differential backup.
- Transaction log backup every 10-15 minutes (Full recovery model).

### 1.2 Backup frequency

Suggested baseline schedule:

- Full DB backup: once per week (off-peak window).
- Differential DB backup: every day.
- Transaction log backup: every 10-15 minutes.
- Configuration files backup: every day and before each release.
- User data backup: every day (or every 6-12 hours for high-change workloads).
- System logs backup: every day.

### 1.3 Storage and rotation

1. Use at least 3-2-1 rule:

- 3 copies of data.
- 2 different storage types.
- 1 offsite/offline copy.

2. Retention (example policy):

- Full backups: 8 weeks.
- Differential backups: 14 days.
- Transaction log backups: 3-7 days.
- Config/userdata/log archives: 14-30 days.

3. Security requirements:

- Encrypt backups at rest.
- Restrict access by least privilege.
- Store backup credentials in a secret manager, not in scripts.

## 2. Backup Procedure

### 2.1 Database backups

1. Set SQL access variables on backup host:

- `DB_SERVER`
- `DB_NAME`
- `DB_USER` / `DB_PASSWORD` (or integrated auth)

2. Run backup script:

Linux/macOS:

```bash
./scripts/backup/backup-prod.sh --type=full
./scripts/backup/backup-prod.sh --type=diff
./scripts/backup/backup-prod.sh --type=log
```

Windows:

```bat
scripts\backup\backup-prod.bat --type=full
scripts\backup\backup-prod.bat --type=diff
scripts\backup\backup-prod.bat --type=log
```

3. Confirm output contains:

- backup file path
- successful `RESTORE VERIFYONLY`

### 2.2 Configuration files backup

Include:

- `Teachio.WebApi/appsettings*.json` (or deployed equivalents)
- service configuration (systemd unit / IIS app pool settings export)
- reverse proxy config (Nginx/IIS rewrite/proxy config)

The provided scripts archive appsettings and service config path if present.

### 2.3 User data backup

Include runtime user content directory (example default used in scripts):

- `/move-it-storage` (Linux)
- `C:\move-it-storage` (Windows)

If your environment uses a different path, set:

- `USER_DATA_DIR=<path>`

### 2.4 System logs backup

Include service and platform logs:

- API logs directory
- reverse proxy logs
- optional journald/event logs export

The provided scripts archive `LOG_SOURCE_DIR` and rotate old archives.

## 3. Backup Integrity Verification

Perform verification for each backup run:

1. DB verification

- Ensure SQL backup completes without errors.
- Ensure `RESTORE VERIFYONLY` succeeds.

2. File archive verification

- Validate archive can be listed/unpacked.
- Validate expected files are present.

3. Metadata verification

- Record timestamp, backup type, source host, DB name, and artifact size.

4. Periodic restore drill

- At least monthly, restore backup to staging and run smoke checks.

Release gate:

- Backup is considered valid only after integrity checks pass.

## 4. Backup Automation (Scripts and Tools)

### 4.1 Included scripts in this repository

- `scripts/backup/backup-prod.sh`
- `scripts/backup/backup-prod.bat`

Features:

- DB backup modes: `full`, `diff`, `log`.
- Config/userdata/logs archiving.
- Built-in SQL `RESTORE VERIFYONLY` check.
- Retention cleanup by file age.

### 4.2 Scheduling examples

Linux (cron examples):

```cron
# weekly full backup (Sunday 02:00)
0 2 * * 0 /opt/teachio-api/scripts/backup/backup-prod.sh --type=full

# daily differential backup (Mon-Sat 02:00)
0 2 * * 1-6 /opt/teachio-api/scripts/backup/backup-prod.sh --type=diff

# transaction log backup every 15 minutes
*/15 * * * * /opt/teachio-api/scripts/backup/backup-prod.sh --type=log
```

Windows:

- Use Task Scheduler with `scripts\backup\backup-prod.bat` and appropriate arguments.

### 4.3 Recommended tooling

- SQL Server Agent jobs for DB backup orchestration.
- System scheduler (cron/Task Scheduler) for file backups.
- Centralized backup storage (S3-compatible bucket, NAS, immutable storage).
- Monitoring/alerting for backup failures.

## 5. Restore Procedure from Backups

## 5.1 Full system restore

Use when full environment is unavailable or heavily corrupted.

1. Stop API service and isolate traffic.
2. Restore database:

- latest full backup
- latest differential backup (if used)
- required transaction logs up to recovery point

3. Restore configuration archives.
4. Restore user data archive.
5. Restore required log archives (optional for forensic investigation).
6. Start API service.
7. Run smoke tests for core business flows.

## 5.2 Selective data restore

Use when only one component is damaged.

1. DB-only restore:

- Restore specific DB to point-in-time on staging first.
- Validate data.
- Apply controlled recovery to production.

2. Config-only restore:

- Restore only required config files.
- Reload service/reverse proxy.

3. User-data-only restore:

- Restore required folders/files from archive.
- Verify permissions and ownership.

## 5.3 Restore testing

1. Run quarterly restore rehearsal in staging:

- Execute full restore from backup chain.
- Measure RTO (recovery time objective) and RPO (recovery point objective).

2. Validate post-restore checks:

- API health and startup logs.
- DB connectivity and schema version.
- One read and one write business scenario.

3. Document outcome:

- restored backup set IDs
- total recovery time
- deviations and corrective actions

## Operational Checklist (Quick)

1. Backup completed without script errors.
2. DB `VERIFYONLY` passed.
3. Archive files exist and are readable.
4. Rotation did not remove required retention windows.
5. Alerts are configured for backup job failures.
