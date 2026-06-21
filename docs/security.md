# Security

Handbook version: **v1.3.1**

## Authentication

| Field | Value |
|-------|-------|
| Provider | None for pilot |
| Integration | Not applicable — trusted local desktop on user machine |
| Future | Windows authentication or role-based access control |

---

## Authorization

| Field | Value |
|-------|-------|
| Roles | Not applicable for pilot |
| Claims | Not applicable |
| Policies | All local users of the application have full access to configured project roots |

---

## Secrets

| Secret | Storage | Rotation |
|--------|---------|----------|
| OpenAI-compatible API key | User-entered in Settings → persisted in local SQLite (not in repo) | User-managed |
| Azure OpenAI API key | User-entered in Settings → persisted in local SQLite | User-managed |
| Azure OpenAI endpoint / deployment names | Settings / SQLite | User-managed |

Secrets are **never** committed to the repository or written to debug logs.

For production, consider Windows Credential Manager or enterprise secret distribution.

---

## Data Protection

| Data Type | Classification | Retention | Encryption |
|-----------|----------------|-----------|------------|
| Customer inquiry input | Confidential (user-entered) | Session + export; not logged by default | At rest: local disk only |
| Historical project PDFs | Confidential (customer IP) | Remain on source file share; referenced in place | Depends on host file system |
| Extracted PDF text in SQLite | Confidential | Until rescan or app folder deleted | Local SQLite file |
| Embedding vectors | Internal | Until rescan or app folder deleted | Local SQLite file |
| API keys | Secret | Until user clears Settings | Local SQLite file |
| Debug logs | Technical metadata only | Until user deletes app folder | Local files |

**Personal data removal:** Delete the application folder (`quotation-accelerator.db`, logs, config). Source project folders are not modified by the application.

---

## GDPR and Privacy

| Requirement | Implementation |
|-------------|----------------|
| Privacy by default | No external transmission without explicit hosted-provider consent |
| Data minimisation | No inquiry/document/prompt logging by default (NFR-006) |
| Transparency | Disclosure text before first hosted AI request |
| Synthetic demo data | Repository contains only sample projects |
| Live demos | User responsibility when using real customer data |

---

## Audit Logging

| Event | Logged (pilot) | Retention |
|-------|----------------|-----------|
| Authentication | N/A | — |
| Hosted AI consent granted | Optional debug metadata only | App folder |
| Inquiry / document content | **No** (default) | — |
| Provider / model / duration | Optional debug mode | App folder |

Full audit trail deferred to production implementation.

---

## Dependencies

| Control | Approach |
|---------|----------|
| Vulnerability scanning | GitHub Dependabot + CI (`dotnet list package --vulnerable`) |
| Update policy | Apply security patches to .NET SDK and NuGet dependencies in CI |

---

## AI Usage

| Topic | Policy |
|-------|--------|
| External LLM usage | Optional; OpenAI-compatible and Azure OpenAI only after user confirmation |
| Preferred path | Ollama local (`qwen3:8b`, `nomic-embed-text`) — data stays on machine |
| Data sent externally | Inquiry fields and extracted document text when hosted provider is used |
| User notification | Settings disclosure + consent dialog; AI Provider Status card |
| Cost exposure | User-provided API keys; no platform-funded hosted inference |

See Heckel ADR-014, ADR-015 and project `adrs/ADR-003-ai-provider-integration.md`.

---

## Document History

| Version | Date | Change |
|---------|------|--------|
| 0.1 | 2026-06-21 | Initial security document from requirements and architecture |
