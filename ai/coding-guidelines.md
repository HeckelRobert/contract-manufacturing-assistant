# Coding Guidelines

Project-specific coding guidance for AI assistants and developers.

Platform defaults are in the Heckel Platform handbook `standards/coding-standard.md`.

---

## Overrides

None.

---

## Module Boundaries

| Module | May depend on | Must not depend on |
|--------|---------------|-------------------|
| SharedKernel | — | Infrastructure, Desktop, other modules |
| Catalog, Inquiry, Matching, Proposal, Documents, Export, Settings | SharedKernel; Matching may use Catalog | Infrastructure, Desktop |
| Infrastructure | SharedKernel, Catalog (interfaces) | Desktop |
| Desktop | Application modules, Infrastructure (composition root only) | — |

View models must call application logic through `IDispatcher`, not infrastructure types directly.

---

## Naming

- Display name: **Contract manufacturing**
- Technical identifiers: **QuotationAccelerator** (namespaces, solution, ZIP file names)
- Executable: `Contract manufacturing.exe`

---

## Patterns Used in This Project

* Internal `IDispatcher` with `ICommand` / `IQuery` handlers (Heckel ADR-002)
* FluentValidation for input validation (Heckel ADR-003)
* Mapster for non-trivial mapping (Heckel ADR-001)
* `Result` / `Result<T>` for business validation outcomes
* MVVM in WPF presentation layer (project ADR-001)
