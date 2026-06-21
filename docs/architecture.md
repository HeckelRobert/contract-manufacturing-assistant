# Architecture

## Overview

{Brief system description and handbook version reference}

---

## Architecture Style

Modular Monolith (default) — see ADR-009

Organization: Vertical Slices per business module — see ADR-010

---

## Modules

| Module | Responsibility | Dependencies |
|--------|----------------|------------|
| | | |

---

## Layering

Per ADR-011: Domain, Application, Presentation, Infrastructure as logical concerns.

---

## Technology Decisions

| Area | Choice | ADR / Reference |
|------|--------|-----------------|
| Backend | Current .NET LTS (ASP.NET Core) | ADR-013 |
| Database | | ADR-012 |
| Authentication | | ADR-004 |
| Frontend | | `standards/frontend-technology-selection.md` |
| Hosting | | |
| AI (if applicable) | | ADR-006, ADR-014, ADR-015 |

---

## .NET version policy

Requirements specify **.NET LTS**, not a fixed major version. At implementation:

1. Confirm active LTS from [official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy).
2. Pin SDK in `global.json` and `TargetFramework` in projects.
3. Align Azure App Service (or host) runtime major version.

---

## External Integrations

| System | Purpose | Protocol |
|--------|---------|----------|
| | | |

---

## Health and observability

| Endpoint | Purpose |
|----------|---------|
| `/health/live` | Liveness |
| `/health/ready` | Readiness |

Document App Service health check path in `docs/operations.md`. See `standards/observability-standard.md`.

---

## Diagrams

{Optional: C4 context, container diagram, or sequence diagrams for critical flows}
