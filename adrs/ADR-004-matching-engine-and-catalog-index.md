# ADR-004: Matching Engine and Catalog Index

## Status

Accepted

---

## Context

Core product value is finding the top three similar historical projects (FR-008) using:

1. **Rule-based** matching (always available)
2. **AI-assisted** matching (embeddings and/or LLM text comparison)
3. **Hybrid** matching (rules → embeddings → LLM → rules fallback)

The catalog is built from a single active project root (FR-003) with `metadata.json` as primary structured source (FR-002). Rescan rebuilds the index. Hybrid must not fail when embeddings are unavailable (FR-007).

---

## Decision

### Catalog indexing

On startup and **Rescan Projects**:

1. Scan active project root for `PRJ-*` folders
2. Parse and validate `metadata.json`
3. Index document filenames and optional extracted PDF text into SQLite
4. Optionally compute and store embedding vectors when Ollama embedding model is available

### Matching

Expose `IMatchingStrategy` implementations:

| Implementation | Behaviour |
|----------------|-----------|
| `RuleBasedMatchingStrategy` | Weighted score from metadata fields, process overlap, keywords, filename hints |
| `AiAssistedMatchingStrategy` | Semantic similarity via embeddings; fallback to LLM pairwise comparison of summaries |
| `HybridMatchingStrategy` | Pre-filter top N by rules → re-rank by embeddings if available → else LLM re-rank → else rule order |

`MatchingService` orchestrates strategy selection from Settings and returns exactly three `ProjectMatchResult` items with percentage score and localized explanation lines.

### Proposal generation

`ProposalService` uses the **primary match** only for manufacturing steps and quotation draft. Second and third matches appear on Results and PDF export for context only (FR-012).

---

## Rationale

Benefits:

* Clear separation for demos comparing strategies
* Rule-based path works with zero AI setup
* Hybrid fallback chain matches requirements without runtime failures
* SQLite index avoids re-parsing PDFs on every inquiry

---

## Alternatives Considered

### In-memory index only (no SQLite)

Advantages

* Simpler first build

Disadvantages

* Slow rescan and repeated PDF extraction on each inquiry

Decision: **Rejected** — SQLite cache preferred (ADR-002).

### Vector database (Qdrant, Redis)

Advantages

* Optimized similarity search at scale

Disadvantages

* Unjustified operational complexity for 8–100 project pilot

Decision: **Rejected** for pilot.

### LLM-only matching

Advantages

* Simple conceptually

Disadvantages

* Cannot demo rule-based comparison; slower and costlier

Decision: **Rejected** as sole approach.

---

## Consequences

Projects should:

* Prefer `metadata.json` over PDF inference for rule-based scoring
* Localize explanation templates in Application layer (FR-019)
* Highlight primary match in Results and PDF export (FR-020)

Projects must not:

* Auto-merge quotations from multiple matches (FR-012)
* Write generated content back to project folders

---

## Review Date

When catalog exceeds low thousands of projects or embedding dimensionality requires dedicated vector store.

---

## Related Documents

* `docs/requirements.md` — FR-002, FR-007, FR-008, FR-012
* `adrs/ADR-002-sqlite-local-persistence.md`
* `adrs/ADR-003-ai-provider-integration.md`
