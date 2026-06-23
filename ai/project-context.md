# Project Context

## Handbook Version

Handbook Version: v1.3.1

---

## Project Name

QuotationAccelerator

**Application display name:** Contract manufacturing

---

## Purpose

Portable Windows desktop prototype that demonstrates how historical manufacturing project knowledge — drawings, contract manufacturing jobs, work instructions, fixtures, and CNC programs — can accelerate technical review and contract manufacturing preparation for contract manufacturing according to technical drawings.

The application answers: **"Have we manufactured something similar before?"**

---

## Solution Type

* [ ] Web Application
* [x] Desktop Application
* [ ] Mobile Application
* [ ] API
* [ ] Background Service
* [ ] Autonomous Agent
* [ ] Hybrid

---

## Key Modules

| Module | Responsibility |
|--------|----------------|
| Desktop | WPF shell, four tabs, MVVM presentation |
| Catalog | Project discovery, `metadata.json`, rescan, SQLite index |
| Inquiry | Customer inquiry capture and validation |
| Matching | Rule-based, AI-assisted, hybrid top-3 search |
| Proposal | Manufacturing steps and contract manufacturing draft generation |
| Documents | Referenced files, PDF preview, open folder |
| Export | Clipboard and contract manufacturing PDF export |
| Settings | Project root, AI providers, language, status card |
| Infrastructure | SQLite, file I/O, AI adapters, PDF libraries |

---

## Technology Summary

| Area | Choice |
|------|--------|
| Runtime | .NET 10 LTS (confirm at scaffold) |
| Desktop UI | WPF + MVVM (`CommunityToolkit.Mvvm`) |
| PDF preview | WebView2 |
| Database | SQLite (`quotation-accelerator.db`) |
| AI | Ollama-first; OpenAI-compatible and Azure OpenAI optional |
| Authentication | None (pilot) |
| Hosting | Portable ZIP — local Windows only |

---

## Local Development

Determine current LTS from the [official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy); ensure `global.json` matches before building.

**Prerequisites (development):**

- Windows 10 or 11
- .NET 10 SDK (or current LTS confirmed at scaffold)
- WebView2 runtime
- Ollama (optional, for AI-assisted/hybrid demos)

```bash
dotnet --version
dotnet restore
dotnet build
dotnet test
```

**Run (after scaffold):**

```bash
dotnet run --project src/Desktop
```

---

## Important Conventions

* Modular monolith with vertical slices (ADR-009, ADR-010)
* Internal command/query dispatcher (ADR-002)
* FluentValidation for input validation (ADR-003)
* Mapster for non-trivial mapping (ADR-001)
* Project ADRs in `adrs/` for desktop, SQLite, AI, and matching decisions

---

## Document Index

* Requirements: `docs/requirements.md`
* Architecture: `docs/architecture.md`
* Operations: `docs/operations.md`
* Security: `docs/security.md`
* ADRs: `adrs/`
