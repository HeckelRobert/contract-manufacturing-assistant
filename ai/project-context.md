# Project Context

## Handbook Version

Handbook Version: v1.3.1

---

## Project Name

{Project Name}

---

## Purpose

{One paragraph describing what this solution does}

---

## Solution Type

* [ ] Web Application
* [ ] Desktop Application
* [ ] Mobile Application
* [ ] API
* [ ] Background Service
* [ ] Autonomous Agent
* [ ] Hybrid

---

## Key Modules

| Module | Responsibility |
|--------|----------------|
| {ModuleName} | {Brief description} |

---

## Technology Summary

| Area | Choice |
|------|--------|
| Backend | Current .NET LTS — pin at implementation (ADR-013) |
| Frontend | {Blazor / Angular / none} |
| Database | {SQL Server / PostgreSQL / other} |
| Authentication | {Entra ID / other} |
| Hosting | {Azure / on-premise / other} |

---

## Local Development

Determine current LTS from the [official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy); ensure `global.json` matches before building.

```bash
dotnet --version
dotnet restore
dotnet build
dotnet test
```

---

## Important Conventions

* Modular monolith with vertical slices (ADR-009, ADR-010)
* Internal command/query dispatcher (ADR-002)
* FluentValidation for input validation (ADR-003)
* Mapster for non-trivial mapping (ADR-001)

---

## Document Index

* Requirements: `docs/requirements.md`
* Architecture: `docs/architecture.md`
* Operations: `docs/operations.md`
* Security: `docs/security.md`
* ADRs: `adrs/`
