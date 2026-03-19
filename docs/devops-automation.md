# DevOps Automation and Infrastructure Pack

This document describes automation artifacts added for deployment/update, CI/CD, containerization, orchestration, and Terraform infrastructure provisioning.

## 1. Deployment and Update Automation Scripts

Location: `scripts/deploy`

- `deploy-prod.sh` / `deploy-prod.ps1`
- `update-prod.sh` / `update-prod.ps1`

Capabilities:

- restore/build/publish for `Teachio.WebApi`
- optional EF migration execution
- release folder strategy (`/opt/teachio-api/releases/<release-id>`)
- switch active release (`/opt/teachio-api/current`)
- service restart and health check (`teachio-api`)
- update script includes basic rollback to previous release if service does not start

Examples:

```bash
./scripts/deploy/deploy-prod.sh
./scripts/deploy/update-prod.sh 20260319-210000
```

```powershell
pwsh ./scripts/deploy/deploy-prod.ps1
pwsh ./scripts/deploy/update-prod.ps1 -ReleaseId "20260319-210000"
```

## 2. CI/CD Configurations

Primary platform: GitHub Actions.

### GitHub Actions

Location:

- `.github/workflows/ci.yml`
- `.github/workflows/cd.yml`

Pipeline behavior:

- CI: restore, build, test, format-check
- CD: build and push Docker image to GHCR on tags (`v*`) and manual trigger

### Jenkins

Location:

- `Jenkinsfile`

Pipeline behavior:

- checkout, restore, build, test
- docker build for `main` branch

## 3. Containerization (Docker and Compose)

Location:

- `Dockerfile`
- `.dockerignore`
- `docker-compose.yml`

Details:

- multi-stage Docker build for .NET 9
- API exposed on port 8080
- local SQL Server service in compose
- connection string wired via environment variable

Local run example:

```bash
docker compose up --build
```

## 4. Container Orchestration

### Kubernetes

Location: `deploy/k8s`

Files:

- `namespace.yaml`
- `configmap.yaml`
- `secret-example.yaml`
- `deployment.yaml`
- `service.yaml`

Apply example:

```bash
kubectl apply -f deploy/k8s/namespace.yaml
kubectl apply -f deploy/k8s/configmap.yaml
kubectl apply -f deploy/k8s/secret-example.yaml
kubectl apply -f deploy/k8s/deployment.yaml
kubectl apply -f deploy/k8s/service.yaml
```

### Docker Swarm

Location:

- `deploy/swarm/docker-stack.yml`

Deploy example:

```bash
docker stack deploy -c deploy/swarm/docker-stack.yml teachio
```

## 5. Terraform Infrastructure Provisioning

Location: `deploy/terraform/aws`

Files:

- `versions.tf`
- `variables.tf`
- `main.tf`
- `outputs.tf`
- `terraform.tfvars.example`

Provisioning flow:

```bash
cd deploy/terraform/aws
cp terraform.tfvars.example terraform.tfvars
terraform init
terraform plan
terraform apply
```

Provisioned baseline:

- VPC
- public subnet + route table + internet gateway
- security group (HTTPS + SSH)
- EC2 instance for API host

## Notes

- Replace placeholders (`OWNER`, image tags, `ami_id`, credentials, hostnames) before production use.
- Keep secrets in CI/CD secret managers, not in repository files.
- Review security groups, TLS, and backup policies according to your organization standards.
