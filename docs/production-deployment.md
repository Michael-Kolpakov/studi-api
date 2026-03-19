# Production Deployment Guide

This document describes how to deploy Teachio API to a production environment.

## Scope and Assumptions

- Deployment target: one API server and one SQL Server instance.
- Application: ASP.NET Core Web API (.NET 9).
- Database: Microsoft SQL Server.
- TLS termination: reverse proxy (Nginx or IIS).

## 1) Hardware Requirements

### API Server

| Profile     | CPU    | RAM  | Disk       | Architecture |
| ----------- | ------ | ---- | ---------- | ------------ |
| Minimum     | 2 vCPU | 4 GB | 30 GB SSD  | x64          |
| Recommended | 4 vCPU | 8 GB | 60+ GB SSD | x64          |

### Database Server

| Profile     | CPU     | RAM    | Disk        | Architecture |
| ----------- | ------- | ------ | ----------- | ------------ |
| Minimum     | 2 vCPU  | 8 GB   | 80 GB SSD   | x64          |
| Recommended | 4+ vCPU | 16+ GB | 150+ GB SSD | x64          |

Notes:

- Keep API and DB on separate disks or separate hosts for better stability.
- Plan extra disk space for SQL backups and logs.

## 2) Required Software

### API Host

- OS: Ubuntu 22.04 LTS/24.04 LTS or Windows Server 2022.
- .NET 9 ASP.NET Core Runtime.
- Reverse proxy:
  - Linux: Nginx.
  - Windows: IIS (ANCM) or Nginx.
- Optional but useful tools: PowerShell 7+, curl, OpenSSL.

### Database Host

- Microsoft SQL Server 2019+ (2022 recommended).
- SQL Server tools (SSMS and/or sqlcmd).

## 3) Network Setup

- Configure DNS record for API host (example: api.example.com).
- Open inbound ports:
  - 443/TCP (required)
  - 80/TCP (for HTTP to HTTPS redirect and ACME challenges, optional)
- Keep Kestrel private (example internal port 5000); do not expose it publicly.
- Allow API host outbound access to SQL Server on 1433/TCP.
- Restrict SQL Server inbound rules to API host IP(s) only.
- Install valid TLS certificate for the public hostname.

## 4) Server Configuration

## 4.1 Create service user and directories

Linux example:

```bash
sudo useradd --system --no-create-home --shell /usr/sbin/nologin teachio
sudo mkdir -p /opt/teachio-api/current
sudo mkdir -p /var/log/teachio-api
sudo chown -R teachio:teachio /opt/teachio-api /var/log/teachio-api
```

## 4.2 Configure environment variables

Set production settings via environment variables or secure secret store.

Required:

- ASPNETCORE_ENVIRONMENT=Production
- ConnectionStrings\_\_DefaultConnection=<production-sql-connection-string>

Linux example:

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Server=<db-host>;Database=teachio-db;User Id=<user>;Password=<password>;TrustServerCertificate=False;Encrypt=True;"
```

## 4.3 Configure process manager

Use systemd on Linux or Windows Service on Windows.

Minimal systemd unit example:

```ini
[Unit]
Description=Teachio API
After=network.target

[Service]
WorkingDirectory=/opt/teachio-api/current
ExecStart=/usr/bin/dotnet /opt/teachio-api/current/Teachio.WebApi.dll
Restart=always
RestartSec=5
User=teachio
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ConnectionStrings__DefaultConnection=Server=<db-host>;Database=teachio-db;User Id=<user>;Password=<password>;TrustServerCertificate=False;Encrypt=True;

[Install]
WantedBy=multi-user.target
```

## 4.4 Configure reverse proxy

- Terminate TLS at proxy.
- Forward traffic to http://127.0.0.1:5000.
- Preserve forwarded headers (X-Forwarded-For, X-Forwarded-Proto).
- Enforce HTTPS redirect.

## 5) Database Configuration

## 5.1 Create database and login

- Create production database (example: teachio-db).
- Create dedicated SQL login/user for the API.
- Grant least privileges required for runtime and migrations.

## 5.2 Apply migrations

Recommended production flow:

- Run migrations in a controlled deploy step before app restart.

Command:

```bash
dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
```

Important:

- The app can auto-apply migrations on startup, but controlled migrations are safer for production change management.
- Take a full backup before schema changes.

## 5.3 Backup and restore policy

- Full backup daily.
- Transaction log backup every 10-15 minutes (if using Full recovery model).
- Keep restore test procedure documented and validated regularly.

Detailed backup/restore process and scripts are documented in:

- [backup-and-restore.md](backup-and-restore.md)

## 6) Code Deployment

## 6.1 Build artifact (CI/CD recommended)

```bash
dotnet restore Teachio.sln
dotnet build Teachio.sln -c Release --no-restore
dotnet test Teachio.sln -c Release --no-build
dotnet publish Teachio.WebApi/Teachio.WebApi.csproj -c Release -o ./artifacts/teachio-api
```

## 6.2 Release steps on server

1. Upload artifact to target host.
2. Stop API service.
3. Replace files in /opt/teachio-api/current.
4. Apply DB migration (if not done earlier in pipeline).
5. Start API service.
6. Check logs and health/smoke endpoints.

## 6.3 Rollback strategy

- Keep previous artifact version.
- If release fails: stop service, restore previous artifact, restart service.
- If migration is non-backward-compatible, use approved DB rollback or restore plan.

## 7) Operational Verification

Use this checklist after each deployment:

1. Process and port checks

- Service status is active/running.
- Reverse proxy responds on HTTPS.

2. API smoke checks

- Open /swagger (if enabled in target environment).
- Execute one read endpoint and one write endpoint with expected responses.

3. Database checks

- API can connect to SQL Server.
- Latest migration appears in \_\_EFMigrationsHistory.

4. Logs and errors

- No startup exceptions in logs.
- No repeated 5xx errors after first traffic.

5. Functional checks

- Main business flow works end-to-end (for example: create/get/update/delete course entities).

6. Monitoring checks

- CPU, memory, and disk are within normal range.
- Alerting channels are active.

## Suggested Production Run Command (Manual)

If running manually for diagnostics:

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet Teachio.WebApi.dll
```

Use service manager for normal production operation.

## 7.1 Automation scripts for routine run operations

For routine startup automation, use scripts from the repository root:

Linux/macOS:

```bash
./scripts/run/run-dev.sh
./scripts/run/run-prod.sh
```

Windows:

```bat
scripts\run\run-dev.bat
scripts\run\run-prod.bat
```

Supported optional arguments:

- `--skip-restore`
- `--skip-migrate`
- `--no-watch` (dev scripts only)
- `--skip-build` (prod scripts only)

Deployment/update automation scripts are documented in:

- [devops-automation.md](devops-automation.md)

## 8) Production Update Runbook (Release Engineer / DevOps)

This section describes a strict update procedure for an existing production installation.

### 1. Preparation for Update

#### 1.1 Create backups

1. Take a full SQL backup of the production database.
2. If Full recovery model is used, take a transaction log backup immediately before update.
3. Archive the currently running API artifact (or keep a symlinked previous release directory).
4. Export current runtime configuration (environment variables, proxy config, service unit).
5. Verify backup integrity by checking file size, timestamp, and backup completion status.

Release gate: do not continue until all backups are confirmed.

#### 1.2 Compatibility check

1. Review release notes and migration scripts for breaking changes.
2. Validate .NET runtime compatibility on target host (runtime version must satisfy build target).
3. Validate SQL Server version compatibility with new EF migrations.
4. Verify required environment variables for the new version are present.
5. Confirm reverse proxy settings remain valid (headers, TLS, upstream port).

Release gate: do not continue if schema changes are incompatible with rollback policy.

#### 1.3 Downtime planning (if needed)

1. Classify deployment mode:

- Rolling/blue-green: near-zero downtime.
- In-place update: maintenance window required.

2. If in-place update is used, announce maintenance window to stakeholders.
3. Freeze non-essential production changes during the update window.
4. Prepare a rollback owner and decision deadline (for example, rollback if not healthy within 15 minutes).

### 2. Update Process

#### 2.1 Stop required services

1. Put API behind maintenance mode (if supported) or drain traffic.
2. Stop application service:

```bash
sudo systemctl stop teachio-api
```

3. Confirm process is stopped and upstream no longer forwards traffic.

#### 2.2 Deploy new code

1. Upload the new artifact to a versioned directory (example: `/opt/teachio-api/releases/<version>`).
2. Verify artifact checksum/signature if your pipeline provides it.
3. Update current symlink/path to the new release.
4. Keep previous release directory unchanged for fast rollback.

Example (Linux symlink strategy):

```bash
ln -sfn /opt/teachio-api/releases/<version> /opt/teachio-api/current
```

#### 2.3 Data migration (if needed)

1. Run database migration in controlled mode:

```bash
dotnet ef database update --project Teachio.DAL --startup-project Teachio.WebApi
```

2. Confirm migration completion and check `__EFMigrationsHistory` for latest entry.
3. If migration fails, stop and execute rollback procedure immediately.

#### 2.4 Update configurations

1. Apply new or changed environment variables required by the release.
2. Update service definition (systemd/IIS) only if required by release notes.
3. Reload service manager configuration when unit file changes:

```bash
sudo systemctl daemon-reload
```

4. Start API service:

```bash
sudo systemctl start teachio-api
```

5. Validate service is running:

```bash
sudo systemctl status teachio-api --no-pager
```

### 3. Post-Update Verification

#### 3.1 Functional correctness tests

Run these checks immediately after service start.

1. Service/process checks:

- Service state is `active (running)`.
- Restart counter is stable (no restart loop).

```bash
sudo systemctl status teachio-api --no-pager
```

2. Network and endpoint checks:

- HTTPS endpoint responds with expected status code.
- Reverse proxy successfully forwards to API upstream.

```bash
curl -k -I https://api.example.com/swagger
```

3. Database connectivity checks:

- Application logs contain no SQL connection/authentication errors.
- Last migration exists in `__EFMigrationsHistory`.

4. Business smoke checks:

- Execute at least one read endpoint and one write endpoint.
- Verify response schema and status codes are as expected.
- Verify written entity is readable afterward (read-after-write).

5. Error-path checks:

- Send one known invalid request and confirm API returns expected 4xx response format.
- Confirm there are no unhandled exceptions for this request.

Release gate:

- If any critical smoke check fails, trigger rollback procedure.

#### 3.2 Performance monitoring

Observe the system during the first 15-30 minutes after release.

1. API host metrics:

- CPU trend compared with pre-release baseline.
- Memory growth and potential leak pattern.
- Disk I/O and log growth rate.

2. API behavior metrics:

- Request throughput (RPS) is within expected range.
- P95/P99 latency does not regress beyond agreed threshold.
- 5xx error rate remains near baseline.

3. DB metrics:

- Active connections and blocking/locking behavior.
- Slow queries or deadlock events.
- Transaction log growth anomalies.

4. Alerting checks:

- Ensure monitoring alerts are enabled and routed.
- Confirm no critical alerts remain unacknowledged.

Escalation trigger examples:

- Sustained 5xx increase over baseline.
- P95 latency regression above accepted SLO.
- Repeated OOM/restart events.

#### 3.3 Common issues and resolution

Use this quick triage map during post-release incidents.

1. Service does not start

- Symptoms: `failed` state, immediate crash, restart loop.
- Checks: service logs, runtime version, environment variables, file permissions.
- Actions: fix config/runtime mismatch; restart service; rollback if unresolved quickly.

2. API starts but returns 502/503 via proxy

- Symptoms: proxy error pages, upstream unavailable.
- Checks: proxy upstream target, local Kestrel port binding, firewall rules.
- Actions: fix upstream config/port; reload proxy; validate with local curl.

3. Database migration failed

- Symptoms: deploy step fails, startup exceptions related to schema.
- Checks: migration output, DB permissions, schema locks, disk space.
- Actions: stop rollout; restore service to previous artifact; run rollback plan for DB if needed.

4. Increased 5xx or timeout rate

- Symptoms: error spike after release.
- Checks: application logs, DB wait stats, recent config changes.
- Actions: disable problematic feature flag (if used), scale resources temporarily, rollback if user impact persists.

5. High CPU or memory after update

- Symptoms: degraded response times, OOM risk.
- Checks: process metrics, GC pressure, expensive queries/endpoints.
- Actions: mitigate load, recycle service if safe, open incident and rollback if instability continues.

6. Broken business flow

- Symptoms: key domain operation fails (for example create/update entities).
- Checks: DTO/validation changes, mapping issues, required env vars/secrets.
- Actions: hotfix if low-risk and fast; otherwise rollback and investigate offline.

Incident handling notes:

- Record timeline, impacted version, root cause hypothesis, and actions taken.
- Create follow-up tasks for permanent fix and monitoring improvement.

### 4. Rollback Procedure (Failed Update)

Trigger rollback if startup fails, migration fails, or critical smoke checks fail.

1. Stop current failed service:

```bash
sudo systemctl stop teachio-api
```

2. Re-point current release to previous artifact (or restore previous files):

```bash
ln -sfn /opt/teachio-api/releases/<previous-version> /opt/teachio-api/current
```

3. If a non-backward-compatible migration was applied, execute approved DB rollback plan:

- Either run validated down-migration sequence.
- Or restore DB from pre-update backup and required log backups.

4. Start service with previous version:

```bash
sudo systemctl start teachio-api
```

5. Validate recovered state:

- Service status is active.
- API responds via HTTPS.
- Core business read/write operations work.

6. Record incident details: failed version, root cause hypothesis, rollback timestamp, and follow-up actions.
