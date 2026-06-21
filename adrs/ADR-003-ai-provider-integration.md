# ADR-003: AI Provider Integration

## Status

Accepted

---

## Context

The prototype must support three matching strategies and multiple AI providers (FR-007, FR-014):

- **Ollama** (preferred, local-first)
- **OpenAI-compatible APIs**
- **Azure OpenAI**

Requirements mandate:

- Provider and model selection in Settings
- Auto-detect Ollama; no bundling in installer
- Default chat model `qwen3:8b`; default embedding `nomic-embed-text` when installed
- Hosted provider consent before external transmission (FR-015)
- AI Provider Status card in Settings (FR-018)
- Abstraction per Heckel ADR-006, abuse/cost awareness per ADR-014 and ADR-015

---

## Decision

Implement AI capabilities behind Heckel-aligned abstractions:

| Interface | Responsibility |
|-----------|----------------|
| `IChatCompletionService` | Text generation for ranking, explanations, proposal drafts |
| `IEmbeddingService` | Vector embeddings for semantic search |
| `IAiProviderFactory` | Resolve provider adapters from user settings |
| `IHostedAiConsentService` | Track disclosure and confirmation state |

Concrete adapters:

- `OllamaChatService` / `OllamaEmbeddingService` — HTTP to local Ollama API
- `OpenAiCompatibleChatService` / `OpenAiCompatibleEmbeddingService`
- `AzureOpenAiChatService` / `AzureOpenAiEmbeddingService`

Application slices depend only on interfaces registered in DI at the Desktop host.

---

## Rationale

Benefits:

* Provider swap without changing Matching or Proposal slices
* Demonstrates Ollama-first path while allowing hosted comparison in workshops
* Aligns with platform ADR-006 and project privacy requirements

---

## Alternatives Considered

### Direct SDK coupling (OpenAI SDK only)

Advantages

* Fast initial integration

Disadvantages

* Violates Heckel ADR-006; harder to demo Ollama-first path

Decision: **Rejected**.

### Single bundled local model in installer

Advantages

* Guaranteed out-of-box AI

Disadvantages

* Conflicts with requirements (Ollama auto-detect only; large installer)

Decision: **Rejected**.

---

## Consequences

Projects should:

* Detect Ollama on Settings open and list installed models
* Block hosted requests until user confirms consent
* Surface provider status in Settings card
* Use tiered model defaults per Heckel ADR-015 (`qwen3:8b` for demos)

Projects must not:

* Send document content to hosted providers without confirmation
* Log prompts or responses by default (NFR-006)

---

## Review Date

Before production deployment with mandatory Entra ID or centralized AI gateway.

---

## Related Documents

* `docs/requirements.md` — FR-007, FR-014, FR-015, FR-018
* Heckel ADR-006, ADR-014, ADR-015
* `docs/security.md`
