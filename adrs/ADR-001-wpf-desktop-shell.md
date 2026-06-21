# ADR-001: WPF Desktop Shell

## Status

Accepted

---

## Context

Quotation Accelerator is a Windows-only portable desktop prototype (requirements NFR-001, NFR-002, NFR-003). It must:

- Run on Windows 10 and 11 without administrative privileges
- Provide four primary tabs, embedded PDF preview, and native folder/file dialogs
- Ship as a self-contained portable ZIP with `Quotation Accelerator.exe`
- Support bilingual UI (German default) and a professional industrial theme

The team is .NET-centric per Heckel platform standards. Cross-platform mobile or web deployment is not required.

---

## Decision

Use **WPF** (.NET 10 LTS) as the desktop presentation technology with the **MVVM** pattern (`CommunityToolkit.Mvvm`).

Use **WebView2** for embedded PDF preview in Proposal Workspace.

---

## Rationale

Benefits:

* Mature, stable Windows desktop stack aligned with enterprise manufacturing environments
* Excellent support for MVVM, data binding, and localization (RESX)
* Native folder picker and shell integration for opening project directories
* WebView2 provides reliable in-app PDF rendering without shipping a custom PDF renderer for preview
* Single-process modular monolith — no Electron overhead

---

## Alternatives Considered

### .NET MAUI

Advantages

* Cross-platform; shared .NET skills

Disadvantages

* Additional complexity for a Windows-only pilot
* PDF preview and polish for desktop demos less mature than WPF + WebView2

Decision: **Rejected** for pilot (Windows-only scope).

### WinUI 3

Advantages

* Modern Windows UI stack

Disadvantages

* Smaller ecosystem for industrial/desktop LOB patterns; team familiarity variable

Decision: **Rejected** for pilot simplicity.

### Electron / web shell

Advantages

* Reuse web UI technologies

Disadvantages

* Larger distribution size; conflicts with portable self-contained .NET goals

Decision: **Rejected**.

---

## Consequences

Projects should:

* Keep business logic out of code-behind; use view models and application handlers
* Document WebView2 runtime as a prerequisite in README
* Use WebView2 only for preview; open externally via shell as fallback

Projects must not:

* Introduce ASP.NET Core host solely for UI unless requirements change
* Embed customer-specific branding in the default shell (NFR-011)

---

## Review Date

Before any production multi-platform expansion — revisit MAUI or web-based UI.

---

## Related Documents

* `docs/requirements.md` — FR-017, FR-018, FR-019, NFR-011
* `docs/architecture.md`
* Heckel `standards/frontend-technology-selection.md`
