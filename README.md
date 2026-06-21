# Project Templates

Reference scaffold for new Heckel Platform projects.

Copy or adapt this structure when creating a solution. See `standards/solution-scaffolding-standard.md` and `standards/repository-standard.md`.

---

# Directory Layout

```
src/
  {BusinessModule}/
    {FeatureSlice}/
tests/
  {BusinessModule}.UnitTests/
  {BusinessModule}.IntegrationTests/
  Architecture.Tests/
docs/
  requirements.md
  architecture.md
  operations.md
  security.md
adrs/
ai/
  project-context.md
  coding-guidelines.md
  testing-guidelines.md
infrastructure/
  (when applicable: IaC, containers, deployment scripts)
.github/
  workflows/
  ISSUE_TEMPLATE/
  dependabot.yml
  PULL_REQUEST_TEMPLATE.md
.cursor/
  rules/
```

---

# Presentation Entry Points

Generate only what the solution requires under `src/`:

| Solution type | Typical entry point |
|---------------|---------------------|
| Web | `src/Web/` or `src/Presentation/Web/` |
| API | `src/Api/` |
| Worker | `src/Worker/` |
| Desktop | `src/Desktop/` |
| Agent | `src/Agent/` |

Prefer business modules at the root of `src/` with presentation as a thin layer. See ADR-010 and ADR-011.

---

# Getting Started

1. Copy `templates/docs/` files into project `docs/` and complete them
2. Copy `templates/ai/` files into project `ai/` and complete them
3. Copy `templates/.github/` (workflows, issue templates, dependabot, PR template) and adjust project/solution names
4. Copy `templates/.cursor/rules/` and adapt for the project
5. Record handbook version in `ai/project-context.md`
6. Create initial ADRs for non-default technology choices
7. For Azure App Service hosting, use `standards/azure-web-application-guide.md`

---

# Related Documents

* standards/solution-scaffolding-standard.md
* standards/cicd-standard.md
* standards/azure-web-application-guide.md
* adrs/ADR-009 through ADR-015
