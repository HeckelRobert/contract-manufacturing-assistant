# ADR-002: SQLite Local Persistence

## Status

Accepted

---

## Context

The pilot must:

- Store settings, catalog index, and optional caches locally (NFR-012)
- Support portable deployment — delete folder to uninstall
- Avoid per-user roaming paths or central database servers
- Rebuild catalog on rescan and project root change (FR-003)

Requirements specify `quotation-accelerator.db` and `appsettings.json` colocated with the executable.

---

## Decision

Use **SQLite** as the sole application database, file path:

`{AppDirectory}/quotation-accelerator.db`

Access via **Entity Framework Core** with SQLite provider, or **Dapper** + `Microsoft.Data.Sqlite` if EF overhead is unnecessary. Implementation may choose either at scaffold time; schema ownership remains in Infrastructure.

User-facing settings (project root, language, AI provider, matching strategy) are persisted in SQLite. `appsettings.json` holds non-secret defaults and deployment configuration only.

---

## Rationale

Benefits:

* Zero installation database service — fits portable ZIP model
* Single-file database easy to delete with application folder
* Sufficient for catalog index, extracted text cache, and embedding storage at pilot scale (8–hundreds of projects)
* Well-supported on .NET LTS

---

## Alternatives Considered

### JSON files only

Advantages

* Very simple

Disadvantages

* Poor fit for catalog index, embedding vectors, and concurrent read/write during rescan

Decision: **Rejected** as primary store (may still use JSON for `metadata.json` on disk).

### SQL Server LocalDB / PostgreSQL

Advantages

* Enterprise familiarity

Disadvantages

* Violates portable, no-admin deployment goals

Decision: **Rejected** for pilot.

---

## Consequences

Projects should:

* Create database on first run if missing
* Keep migrations or schema init in Infrastructure layer
* Never store document contents in logs; extracted text in DB is acceptable for local pilot with user discretion

Projects must not:

* Store API keys in `appsettings.json` committed to repository
* Require external database server for pilot operation

---

## Review Date

Before production multi-user or central catalog requirements.

---

## Related Documents

* `docs/requirements.md` — NFR-012, Persistence section
* Heckel ADR-012 (Database and Persistence)
