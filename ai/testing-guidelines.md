# Testing Guidelines

Project-specific testing guidance for AI assistants and developers.

Platform defaults are in `standards/testing-standard.md`.

---

## Test Projects

| Project | Purpose |
|---------|---------|
| `Catalog.UnitTests` | Catalog scanning, metadata parsing, rescan handlers |
| `Architecture.Tests` | Layer and module dependency rules (NetArchTest) |

Additional module test projects should be added as features are implemented (Matching, Proposal, Export).

---

## Conventions

* Test naming: `MethodName_WhenCondition_ThenExpectedBehavior`
* Validation failures: assert on `Result` / `Result<T>`, not thrown exceptions
* Use deterministic test data from `sample-data/`
* Mark integration tests with `[Trait("Category", "Integration")]` when they touch SQLite or file I/O end-to-end

---

## Integration Test Database

Use SQLite file in a temporary directory under `%TEMP%` for integration tests, deleted after test run.

---

## Architecture Test Rules

Enforce per ADR-011:

* Domain assemblies do not reference Infrastructure or Desktop
* Catalog and other slices do not reference Desktop
* Desktop is excluded from architecture test assemblies (composition root)

---

## CI

Build and test on `windows-latest` with .NET 10.x. WPF requires Windows agents.
