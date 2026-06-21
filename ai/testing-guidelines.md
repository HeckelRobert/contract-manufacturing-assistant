# Testing Guidelines

Project-specific testing guidance for AI assistants and developers.

Platform defaults are in `standards/testing-standard.md`.

---

## Test Projects

| Project | Purpose |
|---------|---------|
| `{Module}.UnitTests` | Business rules, handlers, validators |
| `{Module}.IntegrationTests` | Database, HTTP, external services |
| `Architecture.Tests` | Layer and module dependency rules |

---

## Conventions

* Test naming: `MethodName_WhenCondition_ThenExpectedBehavior`
* Validation failures: assert on results or HTTP responses, not exceptions
* Use deterministic test data; avoid static shared mutable state

---

## Integration Test Database

{Describe approach: SQLite in-memory, Testcontainers, shared dev database}

---

## Architecture Test Rules

Enforce per ADR-011:

* Domain does not reference Infrastructure or Presentation
* Presentation does not reference Infrastructure directly
* Modules do not reference unrelated modules
