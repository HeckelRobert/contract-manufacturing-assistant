# Operations

Handbook version: **{version}**

## Environments

| Environment | Purpose | URL / Endpoint |
|-------------|---------|----------------|
| Development | Local | |
| Staging | Pre-production | |
| Production | Live | |

---

## Azure infrastructure progress (if applicable)

See `standards/azure-web-application-guide.md` for the full checklist.

| Step | Status | Notes |
|------|--------|-------|
| Resource group | | |
| Log Analytics + Application Insights (single instance) | | |
| Key Vault | | |
| App Service (Linux, .NET LTS) | | |
| Custom domain + TLS | | |
| Managed Identity RBAC | | |
| Supporting services (e.g. LLM API, email/SMS) | | |

Adapt the service list to `docs/architecture.md` — examples only.

---

## Deployment

### Prerequisites

-

### Deployment Steps

1.

### Rollback

1.

---

## Configuration

| Setting | Description | Secret |
|---------|-------------|--------|
| | | Yes / No |

Key Vault references: `@Microsoft.KeyVault(SecretUri=...)`

---

## Monitoring

| Signal | Tool | Alert |
|--------|------|-------|
| Logs | Application Insights | |
| Metrics | Application Insights | |
| Health | App Service health check → `/health/ready` or documented path | Unhealthy instance |
| AI cost | Azure Cost Management | Budget threshold |

---

## Health checks

| Layer | Configuration |
|-------|----------------|
| Application | `GET /health/live`, `GET /health/ready` (or `/api/v1/health` — document probe path) |
| Azure App Service | Monitoring → Health check → enable after first deploy |

Do not invoke paid LLMs from health probes.

---

## Application Insights

Use **one** Application Insights resource per environment. Link Web App to existing resource — avoid auto-creating a duplicate when enabling on the App Service.

---

## Backup and Restore

### Backup Schedule

### Restore Procedure

---

## Troubleshooting

| Symptom | Likely Cause | Action |
|---------|--------------|--------|
| Duplicate telemetry | Two App Insights resources | Link one; delete orphan |
| Health check failing | App not deployed or wrong path | Deploy; fix path |
| Custom domain / TLS | DNS or cert not validated | Check CNAME; managed certificate |
