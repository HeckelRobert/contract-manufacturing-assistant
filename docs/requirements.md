# Requirements

## Overview

| Field | Value |
|-------|-------|
| Project Name | QuotationAccelerator |
| Application Display Name | Quotation Accelerator |
| Customer | None — generic, reusable demonstration prototype |
| Purpose | Reusable desktop prototype that demonstrates how historical project knowledge can accelerate technical review and quotation preparation for companies offering contract manufacturing according to technical drawings. The prototype simulates the real-world engineering process and answers: **"Have we manufactured something similar before?"** |
| Stakeholders | Engineering management, managing directors, engineering staff, sales/quotation staff, internal presenters |

---

## Business Goals

1. **Demonstrate knowledge reuse** — Convincingly show that previous projects, quotations, drawings, and manufacturing documents can be identified and reused to support the technical review process.
2. **Validate investment case** — Enable a managing director or head of engineering to immediately understand the potential business value and agree that a production implementation should be evaluated.
3. **Compare matching approaches** — Demonstrate rule-based, AI-assisted, and hybrid similarity strategies so stakeholders can evaluate options without committing to a single production design.
4. **Remain generic** — Support demonstrations with different contract-manufacturing companies without customer-specific customisation.

Success is **not** measured by exact time savings in the pilot; success is concept validation and a credible path to a production-ready solution.

---

## Users

| Actor | Description |
|-------|-------------|
| Engineer | Primary user. Performs technical reviews and prepares quotations. Enters inquiries, reviews matches, edits suggestions, opens documents. |
| Sales / quotation staff | Secondary user. Should be able to operate the application without extensive training. |
| Management stakeholder | Observes demonstrations and evaluates business value. May browse similar projects, preview documents, and navigate to project folders. |
| Presenter | Operates the application during workshops and management demos. |

**Usage assumptions**

- One user per machine during the pilot; no collaborative or multi-user features required.
- Future production implementations may assume concurrent users; the prototype does not need to implement this.
- Demonstrations may use prepared sample data, ad-hoc inquiry input, or a combination.

---

## Primary User Workflow

The application shall use **five primary tabs**: **Inbox**, **Inquiry**, **Results**, **Proposal Workspace**, and **Settings**.

```text
Inbox
  │
  ▼  (optional) Continue contract-manufacturing inquiry
Inquiry
  │
  ▼  Analyze Inquiry
Results
  │
  ▼  (top match pre-selected; user may switch)
Proposal Workspace
  ├── Manufacturing Steps
  ├── Suggested Quotation
  ├── Referenced Documents
  └── Document Preview

Settings — accessible at any time
```

| Tab | Purpose |
|-----|---------|
| **Inbox** | Fetch customer emails from a configured Microsoft 365 mailbox, categorize messages, manage an in-app support queue, and continue contract-manufacturing inquiries into **Inquiry**. |
| **Inquiry** | Capture customer inquiry information, optionally reference a local drawing PDF, and start analysis. |
| **Results** | Present the three best historical matches under the page heading **Top 3 Similar Projects**, with similarity scores, short explanations, and access to source project folders. Tab label **Results** is used for management-friendly navigation; technical content remains explicit on the page. |
| **Proposal Workspace** | Single scrollable page with collapsible sections for reviewing, editing, and reusing generated proposal content. |
| **Settings** | Configure project root, rescan projects, matching strategy, and AI provider — frequently changed during demonstrations. |

**Typical workflow**

1. Engineer fetches mail on **Inbox** and reviews categorized messages.
2. For contract-manufacturing inquiries, engineer continues to **Inquiry** with prefilled fields from the email.
3. Engineer enters or adjusts inquiry information on **Inquiry** (manual entry remains supported).
4. Engineer clicks **Analyze Inquiry**; the assistant searches historical projects.
5. Application navigates to **Results** and shows three similar projects under the heading **Top 3 Similar Projects**.
6. The best-matching project is pre-selected automatically; the user may switch to the second or third match.
7. **Proposal Workspace** displays suggested manufacturing steps, quotation information, referenced documents, and document preview based on the selected primary match.
8. User reviews, edits, copies, exports, or sends the proposal as an email reply when the session originated from **Inbox**.

Users may open **Settings** at any time. Users may return to earlier tabs to adjust inquiry input or select a different match on **Results**; switching the primary match refreshes **Proposal Workspace** content from the newly selected project.

---

## Preferred Demonstration Configuration

The prototype shall ship with sensible defaults so presenters can run the recommended demo path with minimal setup. Alternative implementations and configurations remain acceptable where noted in requirements.

| Setting | Preferred default | Notes |
|---------|-------------------|--------|
| Project root | Bundled `sample-data/` | Until user selects another path (FR-003) |
| Matching strategy | **Hybrid** | Rule-based works out of the box without AI |
| AI provider | **Ollama** | When detected locally |
| Chat model | **qwen3:8b** | Auto-selected when installed |
| Embedding model | **nomic-embed-text** | Auto-selected when installed |
| UI language | **German** | English available in **Settings** |

**Recommended demo path:** Hybrid matching + Ollama + `qwen3:8b` + `nomic-embed-text` on bundled sample data.

Presenters comparing strategies may switch to Rule-based or AI-assisted, or configure hosted providers, during workshops.

---

## Functional Requirements

### FR-001 — Project catalog from file share

**Description:** The application shall discover historical manufacturing projects from a configurable local directory using a fixed folder layout and machine-readable metadata.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Each project is represented by a dedicated folder named with a convention such as `PRJ-2024-0178_StainlessSteelEnclosure/`.
- [ ] Each project folder contains a `metadata.json` file as the primary source of structured project information.
- [ ] The application indexes the active project root at startup and exposes a manual **Rescan Projects** action without requiring restart.
- [ ] Missing optional documents in a project folder do not prevent indexing or matching.
- [ ] The repository includes a bundled `sample-data/` directory with representative demo projects.

---

### FR-002 — Project metadata schema

**Description:** Project metadata shall follow a consistent JSON structure suitable for rule-based matching and demonstration.

**Priority:** Must

**Acceptance Criteria:**

- [ ] `metadata.json` supports at minimum: `ProjectNumber`, `Title`, `Material`, `Quantity`, `Processes`, `SurfaceTreatment`, `Customer`.
- [ ] Document filenames follow common conventions where possible, for example: `Drawing.pdf`, `Offer.pdf`, `Calculation.pdf`, `WorkInstruction.pdf`, `Fixture.pdf`, `CncProgram.pdf`, `InspectionReport.pdf`.
- [ ] The application may supplement metadata with information extracted from PDFs and filenames, but structured matching shall prefer `metadata.json`.

**Example metadata:**

```json
{
  "ProjectNumber": "PRJ-2024-0178",
  "Title": "Stainless Steel Enclosure",
  "Material": "1.4301",
  "Quantity": 50,
  "Processes": [
    "Laser Cutting",
    "Bending",
    "Welding",
    "Painting"
  ],
  "SurfaceTreatment": "RAL 7016",
  "Customer": "Sample Customer"
}
```

---

### FR-003 — Configurable project root

**Description:** Users shall configure a single active directory where historical projects are stored. The prototype shall work out of the box using bundled sample data.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Exactly **one active project root** is used at a time.
- [ ] The user can select a local folder or network share on the **Settings** tab.
- [ ] The selected path is persisted and automatically restored on application start.
- [ ] The bundled `sample-data/` directory is always available to the application.
- [ ] If no project root has been configured yet, the application automatically uses bundled `sample-data/` as the active root.
- [ ] The user can switch to another directory at any time; changing the active root triggers an automatic project rescan and rebuilds the catalog for subsequent searches.
- [ ] Manual **Rescan Projects** remains available without restarting the application.

**Typical workflow**

1. User starts the application.
2. If no project root is configured, `sample-data/` is used automatically.
3. User may select another local directory or network share in **Settings**.
4. The application rescans projects automatically.
5. The rebuilt catalog is used for subsequent searches.

---

### FR-004 — Customer inquiry capture

**Description:** Users shall enter inquiry information for a new customer request using a simple form with structured and free-text fields.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Required fields: **Quantity**, **Material**, **Surface Treatment**.
- [ ] Optional fields: **Part Description**, **Delivery Deadline**, **Special Requirements**, **Manufacturing Processes**, **Free-text Notes**.
- [ ] Dimensions and tolerances are out of scope for the pilot.
- [ ] Predefined options are available for common values:
  - Materials: S235, S355, Stainless Steel 1.4301, Stainless Steel 1.4571, Aluminium EN AW-5754
  - Surface treatments: None, Powder Coated, Wet Paint, Galvanized, Anodized
  - Manufacturing processes: Laser Cutting, Bending, Turning, Milling, Welding, Painting, Assembly
- [ ] Special requirements are captured as free text.

---

### FR-005 — Customer drawing reference

**Description:** Users may reference a customer drawing from the local file system without uploading or copying it into the application.

**Priority:** Must

**Acceptance Criteria:**

- [ ] The user can select a local PDF drawing from disk.
- [ ] The application stores only the file path/reference, not a duplicate of the file.
- [ ] Text-only inquiries without a drawing are supported.
- [ ] STEP and IGES support are out of scope for the pilot.

---

### FR-006 — Inquiry analysis trigger

**Description:** Users shall explicitly start analysis of the current inquiry.

**Priority:** Must

**Acceptance Criteria:**

- [ ] A clear action such as **Analyze Inquiry** starts search and suggestion generation.
- [ ] Minimum input to run analysis: **Material** and either a selected drawing PDF or a short part description.
- [ ] Quantity, surface treatment, and manufacturing processes are optional for execution but improve ranking when provided.
- [ ] After analysis completes, the application navigates the user to **Results**.

---

### FR-007 — Similarity matching strategies

**Description:** The application shall support multiple matching strategies selectable at runtime.

**Priority:** Must

**Acceptance Criteria:**

- [ ] The user can select one of: **Rule-based**, **AI-assisted**, **Hybrid** on the **Settings** tab.
- [ ] **Hybrid** is the default matching strategy on first launch.
- [ ] **Rule-based** matching compares structured information such as material, surface treatment, manufacturing processes, quantity, keywords, and file names.
- [ ] **AI-assisted** matching compares textual information extracted from project documents and the customer drawing when available, using embeddings and/or LLM-based text comparison depending on availability.
- [ ] **Hybrid** matching applies rule-based pre-filtering, then ranks candidates using semantic embeddings when available, otherwise LLM-based comparison and ranking.
- [ ] **Hybrid** does not strictly depend on embeddings and shall remain available with graceful fallback (see below).
- [ ] The application remains functional without external network access when a local model is configured.
- [ ] The prototype works out of the box using rule-based matching before any AI provider is configured.
- [ ] **Rule-based** matching is always available.
- [ ] **Hybrid** matching is always selectable and shall not fail at runtime; when embeddings and LLM are unavailable, it degrades to rule-based matching behaviour.
- [ ] **AI-assisted** remains visible in **Settings** but is **disabled** when neither embeddings nor a usable LLM provider (local Ollama or configured hosted provider) is available.
- [ ] When disabled, a tooltip or information message explains why (for example: *"AI-assisted matching requires either embeddings or a configured language model."*).
- [ ] Users are not shown runtime errors for unavailable strategies; unavailable options are disabled with explanation.

**Hybrid fallback order**

1. Rule-based pre-filtering
2. Semantic ranking using embeddings (if available)
3. Otherwise LLM-based comparison and ranking (if available)
4. Otherwise rule-based ranking only

---

### FR-008 — Present results (Top 3 similar projects)

**Description:** After analysis, the application shall present the three most relevant historical projects on the **Results** tab.

**Priority:** Must

**Acceptance Criteria:**

- [ ] The tab label is **Results** for management-friendly navigation.
- [ ] The page displays a prominent heading **Top 3 Similar Projects**.
- [ ] Exactly three ranked results are shown for a successful search with sufficient catalog data.
- [ ] Each result shows a similarity percentage score.
- [ ] Each result shows a short human-readable explanation, for example: "Same material", "Contains welding operations", "Existing bending setup found", "Comparable enclosure assembly".
- [ ] A fully explainable scoring algorithm is not required; percentage plus short rationale is sufficient for the pilot.
- [ ] Results are presented on the **Results** tab.
- [ ] The top-ranked project is pre-selected automatically after analysis.
- [ ] The user can switch the primary match to the second or third project before or while using **Proposal Workspace**.
- [ ] The second and third matches provide context and references but do not automatically influence generated proposal content.

---

### FR-009 — Referenced documents and navigation

**Description:** Users shall access knowledge associated with matched projects directly from the results view.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Each matched project lists referenced documents available in its project folder.
- [ ] Users can open referenced documents with the system default application.
- [ ] Users can open the original project folder in the file explorer.
- [ ] Missing documents are indicated gracefully without breaking the workflow.

---

### FR-010 — In-application PDF preview

**Description:** Users shall preview PDF documents inside the application.

**Priority:** Must

**Acceptance Criteria:**

- [ ] PDF preview is available for project documents and, where applicable, the selected customer drawing.
- [ ] Preview works for bundled sample documents and documents on a configured local file share.
- [ ] Preview failure shows a clear message; the user can still open the file externally.
- [ ] PDF preview is available within **Proposal Workspace** under a **Document Preview** section with an embedded PDF viewer.

---

### FR-011 — Suggested manufacturing steps

**Description:** The application shall propose a manufacturing process chain derived primarily from matched project knowledge.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Steps are derived from matched project documents, structured metadata, and/or pre-authored sample information; LLM-generated summaries are acceptable.
- [ ] Each step includes where available:
  - Process name
  - Estimated effort
  - Related machine
  - Reference document(s)
- [ ] Example step content may include CNC program availability, fixture availability, and external supplier notes.
- [ ] Proposed steps are initially editable by the user.
- [ ] The application does not write changes back to original project files.
- [ ] Manufacturing steps are presented and edited in **Proposal Workspace**.

---

### FR-012 — Quotation template draft

**Description:** The application shall propose reusable quotation information without generating a final commercial offer.

**Priority:** Must

**Acceptance Criteria:**

- [ ] The draft is primarily based on the best-matching project (or the user-selected primary match on **Results**).
- [ ] Information from the second and third matches may be shown as reference context but shall not automatically merge into the generated proposal.
- [ ] The application explains the origin of suggested items, for example: "Derived from Project PRJ-2024-0178" or "Referenced in Project PRJ-2024-0178".
- [ ] Proposed quotation content is initially editable by the user.
- [ ] The application does not write changes back to original project files.
- [ ] ERP export and automatic final quotation generation are out of scope.
- [ ] The suggested quotation is presented and edited in **Proposal Workspace**.

---

### FR-013 — Reuse actions for generated content

**Description:** Users shall be able to reuse generated suggestions during review.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Users can copy generated content to the clipboard.
- [ ] Users can export the current proposal to PDF (see FR-020).
- [ ] Export to Word and direct ERP integration are out of scope for the pilot.
- [ ] Copy, export, and document review actions are available from **Proposal Workspace**.

---

### FR-020 — PDF export

**Description:** Users shall export the current Proposal Workspace content to a PDF working document suitable for internal review and management discussions. The export is not a customer-facing quotation.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Export is triggered from **Proposal Workspace**.
- [ ] Exported content reflects the user's current edits in **Proposal Workspace**.
- [ ] Text follows the selected UI language (FR-019).
- [ ] The PDF does **not** embed document previews from referenced PDF files.

**Header (no separate title page)**

- [ ] Each export includes a simple header with at minimum:
  - **Quotation Accelerator** (application display name)
  - **Proposal Draft** (document type; localized per FR-019)
  - Generation timestamp (for example: `2026-06-21 18:30`)
  - Active search strategy (Rule-based, AI-assisted, Hybrid)
  - Active model name when an AI provider/model was used (for example: `qwen3:8b`)
- [ ] Layout is clean and professional.

**Section order and content**

1. **Inquiry Summary** — original inquiry fields:
   - Part Description
   - Quantity
   - Material
   - Surface Treatment
   - Requested Delivery Date
   - Special Requirements
   - Selected Drawing Filename (if provided)

2. **Top 3 Similar Projects** — for each of the three results:
   - Project Number
   - Project Name
   - Similarity Score (percentage)
   - Short explanation(s) / reasons (similarity explanations)
   - The best-matching (primary) project is visually highlighted

3. **Suggested Manufacturing Steps** — from the primary match, including user edits

4. **Suggested Quotation Information** — from the primary match, including user edits

5. **Referenced Documents** — list of document filenames/paths associated with the primary match

- [ ] Suggested manufacturing steps and quotation information in the export are based on the **primary match** (pre-selected top result or user-selected match on **Results**).
- [ ] Second and third matches appear in the **Top 3 Similar Projects** section for context but do not merge into the suggested sections.

**Example result block (English)**

```text
PRJ-2024-0178 — Stainless Steel Enclosure
94%

Reasons:
- Same material
- Contains welding operations
- Existing bending fixture available
```

---

### FR-014 — AI provider configuration

**Description:** Users shall configure how AI-assisted and hybrid matching, and optional summarisation, are performed. Local Ollama is the preferred provider for demonstrations.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Supported provider types: **Ollama**, **OpenAI-compatible APIs**, **Azure OpenAI**.
- [ ] The user can select provider and model at runtime on the **Settings** tab.
- [ ] Local model usage keeps processing on the local machine.
- [ ] Preferred demonstration setup: local file share, local embedding model, local language model (Ollama).
- [ ] Hosted providers are optional and intended for comparison and evaluation.
- [ ] The application distribution package does **not** install or bundle Ollama.

**Ollama integration (auto-detect only)**

- [ ] On startup and when opening **Settings**, the application detects whether Ollama is running locally.
- [ ] When Ollama is available, locally installed models are discovered automatically and listed for selection.
- [ ] When Ollama is not detected, **Settings** shows a short explanation and link to [https://ollama.com](https://ollama.com), including recommended setup guidance (for example: `ollama pull qwen3:8b`).
- [ ] Example message when Ollama is not detected:

  ```text
  Ollama was not detected on this machine.

  To use local language models, please install Ollama and download a model.

  Recommended command:
  ollama pull qwen3:8b

  More information:
  https://ollama.com
  ```

**Default model selection**

- [ ] When Ollama is detected and `qwen3:8b` is installed, it is selected automatically.
- [ ] If `qwen3:8b` is not available, the first discovered model is selected.
- [ ] The user may choose another model at any time.
- [ ] Suggested demonstration chat models: `qwen3:8b`, `gemma3:12b`, `mistral-small3.1`.

**Local embeddings**

- [ ] No specific embedding technology is prescribed; architecture may choose the most suitable approach.
- [ ] **Preferred approach:** Ollama embedding models when Ollama is available (for example: `nomic-embed-text`, `mxbai-embed-large`).
- [ ] **Fallback for AI-assisted:** if embeddings are unavailable, compare extracted texts directly using the selected LLM.
- [ ] Offline semantic search shall be possible when Ollama and a suitable embedding model are available.
- [ ] Embedding model selection is exposed in **Settings** alongside chat model selection.

**Example Settings layout**

```text
Provider:        Ollama
Chat Model:      qwen3:8b
Embedding Model: nomic-embed-text

Matching Strategy:
○ Rule-based
○ AI-assisted
● Hybrid
```

- [ ] When Ollama is detected, locally installed embedding models are discovered automatically where supported.
- [ ] Suggested default embedding model: `nomic-embed-text` when installed; otherwise first discovered embedding model.
- [ ] **Preferred demonstration path:** Hybrid + Ollama + `qwen3:8b` + `nomic-embed-text`; alternative implementations are acceptable if requirements are met.

---

### FR-015 — Hosted AI consent and disclosure

**Description:** The application shall protect users from unintentionally sending sensitive data to external services.

**Priority:** Must

**Acceptance Criteria:**

- [ ] By default, no project documents, drawings, or extracted document contents leave the local machine.
- [ ] When a hosted provider is selected, the application clearly states that inquiry information and document contents may be transmitted externally.
- [ ] The user must explicitly confirm before the first external AI request in a session or after provider change.
- [ ] Typical demonstration flows should encourage local models whenever possible.

---

### FR-016 — Bundled demonstration dataset

**Description:** The repository shall include realistic synthetic sample projects because no real customer data is available.

**Priority:** Must

**Acceptance Criteria:**

- [ ] The bundled dataset contains **8** representative manufacturing projects.
- [ ] Each project includes `metadata.json` and a realistic subset of PDF documents.
- [ ] No real customer or personal data is included.
- [ ] Sample projects cover varied materials, processes, and document combinations so all three matching strategies can be demonstrated.

**Required sample projects**

| Folder | Title | Material | Qty | Surface | Key processes |
|--------|-------|----------|-----|---------|---------------|
| `PRJ-2019-0142_Stainless-Enclosure` | Stainless Steel Enclosure | 1.4301 | 25 | Powder Coated | Laser Cutting, Bending, Welding, Painting |
| `PRJ-2020-0087_Machine-Housing` | Machine Housing | S355 | 5 | Wet Paint | Laser Cutting, Bending, Welding, Painting |
| `PRJ-2021-0033_Mounting-Bracket` | Mounting Bracket | EN AW-5754 | 200 | Anodized | Laser Cutting, Bending, Milling |
| `PRJ-2018-0201_Welded-Frame` | Welded Frame | S235 | 12 | Galvanized | Laser Cutting, Bending, Welding |
| `PRJ-2022-0115_Hydraulic-Assembly` | Hydraulic Assembly | 1.4571 | 8 | None | Turning, Milling, Assembly |
| `PRJ-2020-0156_Control-Cabinet-Door` | Control Cabinet Door | S355 | 40 | Powder Coated | Laser Cutting, Bending, Painting |
| `PRJ-2021-0099_Shaft-Coupling-Hub` | Shaft Coupling Hub | 1.4301 | 150 | None | Turning, Milling |
| `PRJ-2019-0064_Tank-Support-Frame` | Tank Support Frame | S235 | 6 | Wet Paint | Laser Cutting, Bending, Welding, Painting |

**Suggested demo document set per project**

- `Drawing.pdf` — title block with part name, material, quantity, illustrative dimensions
- `Offer.pdf` — historical quotation with line items and totals
- `Calculation.pdf` — effort and cost breakdown by operation
- `WorkInstruction.pdf` — manufacturing sequence and inspection points
- `Fixture.pdf` — fixture / clamping concept
- `CncProgram.pdf` — program header, tool list, operation summary
- `InspectionReport.pdf` — optional quality record

**Suggested demo inquiry scenarios**

- Stainless steel, powder coated, enclosure-like drawing → top matches: Stainless Enclosure, Control Cabinet Door
- S235, galvanized, frame/bracket description → top matches: Welded Frame, Tank Support Frame
- 1.4571, turning/milling, hydraulic keyword → top matches: Hydraulic Assembly, Shaft Coupling Hub

---

### FR-017 — Proposal Workspace

**Description:** The application shall provide a dedicated **Proposal Workspace** tab where engineers and management stakeholders review, edit, and reuse generated proposal content.

**Priority:** Must

**Acceptance Criteria:**

- [ ] **Proposal Workspace** is reachable after analysis has produced results; content is generated from the pre-selected top match unless the user selects another primary match on **Results**.
- [ ] **Proposal Workspace** is a **single scrollable page** with **collapsible sections or cards** (no nested sub-tabs).
- [ ] Sections appear in this order:
  1. **Manufacturing Steps** — editable ordered process list (for example: Laser Cutting, Bending, Welding, Painting)
  2. **Suggested Quotation** — editable fields including at minimum: Material, Quantity, Surface Treatment, Estimated Setup Effort, Estimated Production Effort, Reference Projects
  3. **Referenced Documents** — list of available files (for example: Calculation.pdf, Offer.pdf, Fixture.pdf, WorkInstruction.pdf) with open-file and open-folder actions
  4. **Document Preview** — embedded PDF viewer for the selected referenced document or customer drawing
- [ ] **Copy to Clipboard** and **Export to PDF** actions are available from **Proposal Workspace**.
- [ ] Switching the primary match on **Results** refreshes workspace content from the newly selected project.
- [ ] Re-running **Analyze Inquiry** from **Inquiry** replaces the current proposal with new results.
- [ ] Management users can browse referenced documents and document preview without engineering-only configuration access.

---

### FR-018 — Settings tab

**Description:** The application shall expose configuration through a dedicated **Settings** tab accessible at any time.

**Priority:** Must

**Acceptance Criteria:**

- [ ] **Settings** is always reachable without completing an inquiry workflow.
- [ ] **Settings** includes at minimum:
  - Single active project root directory selection (local folder or network share)
  - **Rescan Projects** action
  - Matching strategy selection (Rule-based, AI-assisted, Hybrid)
  - AI provider and model configuration (Ollama auto-detect, OpenAI-compatible, Azure OpenAI)
  - Ollama status, model list, and installation guidance when not detected
  - Hosted-provider disclosure and consent management
  - UI language selection (German, English)
  - Optional debug logging toggle
- [ ] **Settings** displays an **AI Provider Status** card summarising current capabilities at a glance.
- [ ] The status card is localized (FR-019) and refreshed when **Settings** is opened and when provider configuration changes.
- [ ] The status card shows availability for at minimum:
  - Rule-based matching
  - Ollama (detected / not detected; selected chat model when available)
  - Embedding model (available / not available; selected model when available)
  - Azure OpenAI (configured / not configured)
  - OpenAI-compatible API (API key present / missing)
- [ ] Each line uses a clear available/unavailable indicator (for example: checkmark and cross icons).
- [ ] Example layout:

  ```text
  AI Provider Status
  ─────────────────────────────

  ✓ Rule-based matching available

  ✓ Ollama detected
    Chat model: qwen3:8b

  ✓ Embedding model available
    Model: nomic-embed-text

  ✗ Azure OpenAI not configured

  ✗ OpenAI API key missing
  ```

- [ ] When Ollama is not detected, the status card links to installation guidance (FR-014).
- [ ] Changes made in **Settings** apply to the next analysis run; changing the active project root rescans the catalog immediately.
- [ ] Switching matching strategy, provider, or language during a demo does not require application restart.

---

### FR-019 — Localization (German and English)

**Description:** The application shall support German and English user interfaces with German as the default language.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Users can select **German** or **English** on the **Settings** tab.
- [ ] **German** is the default language on first launch.
- [ ] The selected language is remembered between application sessions.
- [ ] All user-facing UI text is localized, including:
  - Navigation labels and tab names
  - Buttons and actions
  - **Settings** labels and help text
  - Similarity explanations on **Results**
  - Suggested manufacturing steps and suggested quotation content generated by the assistant
  - Privacy and consent messages
  - Error and status messages
- [ ] Similarity explanations always follow the selected UI language.

**Example similarity explanations**

| German | English |
|--------|---------|
| Gleiches Material | Same material |
| Enthält Schweißarbeiten | Contains welding operations |
| Vorhandene Biegevorrichtung gefunden | Existing bending fixture found |
| Vergleichbare Baugruppe | Comparable assembly |

- [ ] Bundled sample project folders and demonstration PDFs may remain in English, German, or a mixture; localization of source documents is not required for the pilot.
- [ ] When the user changes language, newly generated analysis results use the selected language; already-displayed content refreshes on the next analysis run or primary-match change.

---

### FR-020 — Microsoft 365 mailbox configuration

**Description:** Users shall configure a Microsoft 365 mailbox for fetching and sending inquiry emails.

**Priority:** Must

**Acceptance Criteria:**

- [ ] **Settings** exposes tenant id, application (client) id, mailbox address, and folder name (default `Inbox`).
- [ ] User can connect via Entra ID interactive sign-in (MSAL).
- [ ] User can test connection, disconnect, and reconnect.
- [ ] Mail credentials and refresh tokens are stored locally, not in the repository.

---

### FR-021 — Fetch and display inbox messages

**Description:** Users shall fetch customer inquiry emails on demand from the configured mailbox.

**Priority:** Must

**Acceptance Criteria:**

- [ ] **Inbox** tab provides **Fetch mail** action.
- [ ] Messages display date, sender, subject, category, and attachment indicators.
- [ ] PDF attachments are copied to the application data folder for drawing reference.
- [ ] Non-PDF attachments (e.g. STEP) are listed by filename only; STEP parsing remains out of scope (FR-005).

---

### FR-022 — Email categorization

**Description:** Fetched emails shall be categorized to guide the next action.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Each message is categorized as **AutoAnswerable**, **SupportRequired**, or **ContractManufacturingInquiry**.
- [ ] Rule-based categorization runs without AI; optional AI assist follows existing consent rules (FR-015).
- [ ] Default mail response templates for typical manufacturing client questions are configurable in **Settings**.

---

### FR-023 — In-app support queue

**Description:** Emails requiring human support shall be tracked in an in-app queue.

**Priority:** Must

**Acceptance Criteria:**

- [ ] User can escalate a message to the support queue from **Inbox**.
- [ ] Queue items have status **Open**, **InProgress**, or **Resolved**.
- [ ] User can add notes and update status.

---

### FR-024 — Continue contract-manufacturing inquiry from email

**Description:** Contract-manufacturing inquiry emails shall prefill the **Inquiry** tab.

**Priority:** Must

**Acceptance Criteria:**

- [ ] **Continue inquiry** action navigates to **Inquiry** with prefilled quantity, material, surface, delivery deadline, part description, processes, and drawing path when available.
- [ ] Email subject and sender are preserved in notes for traceability.
- [ ] User can edit all prefilled values before analysis.

---

### FR-025 — Send proposal reply by email

**Description:** Users shall send completed proposals back to the customer by email when the session originated from **Inbox**.

**Priority:** Must

**Acceptance Criteria:**

- [ ] **Proposal Workspace** exposes **Send as email** when a source inbox message is linked.
- [ ] Reply pre-fills recipient and subject; user confirms before send.
- [ ] PDF or Word export can be attached.
- [ ] After send, inbox message status reflects **Replied**.

---

## Non-Functional Requirements

### NFR-001 — Desktop deployment model

**Description:** The solution shall run as a local desktop application on Windows without requiring cloud hosting or a web server.

**Priority:** Must

**Acceptance Criteria:**

- [ ] The application runs fully on a local Windows machine.
- [ ] No cloud deployment or web hosting is required for the pilot.
- [ ] Historical documents remain on a local file share or local disk; the application references them in place.

---

### NFR-002 — Supported operating systems

**Description:** The pilot shall support common business Windows environments.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Supported on Windows 10 and Windows 11.
- [ ] Typical business laptop hardware is sufficient; no specialist workstation is required.

---

### NFR-003 — Packaging and distribution

**Description:** The prototype shall be distributed as a portable, self-contained Windows application. An installer is optional and not required for the pilot.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Primary deliverable is a release ZIP archive named with the pattern `QuotationAccelerator-v{version}-win-x64.zip`.
- [ ] Users can download the archive, extract it to any folder, and start the application immediately without administrative privileges.
- [ ] The release contains a self-contained executable suitable for offline demonstration machines.
- [ ] Application window title displays **Quotation Accelerator**.
- [ ] Application launches and performs rule-based analysis without prior AI configuration.
- [ ] Ollama is **not** bundled with the release package.
- [ ] A formal uninstaller is **not** required; deleting the application folder removes the prototype completely.
- [ ] An optional MSI installer may be added in a future version; MSIX is not required for the pilot.

**Release structure (pilot)**

```text
QuotationAccelerator-v1.0-win-x64.zip
├── Quotation Accelerator.exe
├── sample-data/
├── appsettings.json
├── quotation-accelerator.db
└── README.md
```

**Typical usage**

1. Download ZIP archive.
2. Extract to any local folder.
3. Start `Quotation Accelerator.exe`.
4. Review bundled sample projects.
5. Optionally configure another project directory in **Settings**.
6. Start demonstrating.

---

### NFR-004 — Offline and local-first operation

**Description:** The prototype shall prioritise local execution and remain usable without internet access.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Rule-based matching works with no network connectivity.
- [ ] AI-assisted and hybrid modes work offline when a local model provider is configured.
- [ ] No mandatory cloud dependency exists for core functionality.

---

### NFR-005 — Privacy and data minimisation

**Description:** The prototype shall follow a privacy-first approach aligned with GDPR principles.

**Priority:** Must

**Acceptance Criteria:**

- [ ] External transmission of inquiry or document content occurs only after explicit user confirmation.
- [ ] Repository and bundled data contain synthetic demonstration content only.
- [ ] Users are warned that live demonstrations with real customer data are their responsibility, especially when using hosted AI providers.

---

### NFR-006 — Logging

**Description:** Logging shall be minimal by default and safe for demonstration environments.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Inquiry contents, document contents, prompts, and model responses are not persisted by default.
- [ ] Optional debug logging may be enabled by the user.
- [ ] Debug logs contain only technical metadata such as provider, model name, processing duration, indexed project count, and retrieved document count.
- [ ] Logs do not contain personal information or document contents.

---

### NFR-007 — Authentication

**Description:** The pilot does not require user authentication.

**Priority:** Must

**Acceptance Criteria:**

- [ ] No login is required to use the prototype on a trusted machine.
- [ ] Future production implementations may add Windows authentication or role-based access control; this is out of scope for the pilot.

---

### NFR-008 — Usability

**Description:** The application shall be simple enough for sales and quotation staff while supporting engineering review workflows.

**Priority:** Should

**Acceptance Criteria:**

- [ ] A new user can complete a demonstration flow — enter inquiry, analyse, review results, preview a PDF, open a project folder — without formal training.
- [ ] Management users can browse results and documents without engineering-only configuration steps.
- [ ] Default German UI supports typical German manufacturing demonstrations; English is available for international audiences.

---

### NFR-009 — Backend technology

**Description:** Server-side or application logic shall use the **current .NET LTS** at implementation time (platform ADR-013).

**Priority:** Must

**Acceptance Criteria:**

- [ ] Active LTS confirmed from official .NET support policy before scaffold.
- [ ] SDK pinned in `global.json`; `TargetFramework` set consistently.
- [ ] CI and build toolchain match pinned LTS major version.

---

### NFR-010 — Hosted LLM controls

**Description:** Because hosted AI providers are supported, abuse, disclosure, and cost exposure shall be addressed per platform ADR-014 and ADR-015.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Hosted provider usage requires explicit user confirmation before data leaves the machine.
- [ ] Provider selection, disclosure text, and confirmation flow are documented and testable.
- [ ] Project architecture documents how API credentials are supplied without storing secrets in the repository.

---

### NFR-011 — Branding and visual identity

**Description:** The prototype shall use company-neutral branding suitable for demonstrations with different manufacturing companies.

**Priority:** Must

**Acceptance Criteria:**

- [ ] User-facing name **Quotation Accelerator** (with space) is used consistently in the UI, window title, PDF export header, and release documentation.
- [ ] **QuotationAccelerator** (no space) is reserved for technical identifiers: repository name, namespaces, ZIP archive name, and database filename.
- [ ] The main executable may be named `Quotation Accelerator.exe` in the release package.
- [ ] No company-specific or Heckel branding is included by default.
- [ ] A simple generic manufacturing-related placeholder icon is used (for example: gear, factory, blueprint, or CNC tool); no customer logo.
- [ ] Visual theme is modern, clean, and industrial with:
  - White background
  - Dark gray text
  - Blue accent color
- [ ] Architecture allows future customer-specific branding without rework of core workflows.

---

## Compliance

| Requirement | Reason |
|-------------|--------|
| GDPR-aligned privacy by default | German/European audience; customer drawings and project documents may contain business-sensitive or personal data |
| No real customer data in repository | Avoid legal exposure and unsafe demo defaults |
| Explicit consent before external AI processing | Lawful, transparent processing when hosted models are used |
| User responsibility for live demo data | Presenters may use real inquiries outside the repository; application must warn when external AI is enabled |

---

## Hosting

| Field | Value |
|-------|-------|
| Preferred Hosting | Local Windows desktop — no server hosting |
| Allowed Hosting | Local machine only for the pilot |
| Deployment Responsibility | Project team delivers portable ZIP release; end users extract and run on trusted business laptops without admin rights |

---

### NFR-012 — Portable local data layout

**Description:** Application state shall remain inside the application directory to support simple portable deployment and removal.

**Priority:** Must

**Acceptance Criteria:**

- [ ] Configuration is stored in `appsettings.json` within the application directory (or an equivalent local config file colocated with the executable).
- [ ] Application data, including catalog index and session persistence, is stored in a local SQLite database file `quotation-accelerator.db` within the application directory.
- [ ] Optional debug logs are written inside the application directory when enabled.
- [ ] No application data is written to per-user roaming folders or system directories by default.
- [ ] Deleting the extracted application folder removes configuration, database, and logs completely.

---

## Authentication

| Field | Value |
|-------|-------|
| Preferred | None for pilot |
| Alternatives | Future production: Windows authentication or role-based access control |

---

## Persistence

| Field | Value |
|-------|-------|
| Preferred Persistence | Local SQLite database (`quotation-accelerator.db`) and `appsettings.json` in the application directory |
| Stored data | Active project root path, provider configuration, language, user preferences, project catalog index |
| Alternative Persistence | In-memory session state during runtime |
| Out of scope | Central or server-hosted database, multi-user persistence, writing back to historical project folders |
| Removal | Delete application folder — no uninstaller required |

---

## Messaging

| Requirement | Status |
|-------------|--------|
| Message bus / async integration | Not required |
| ERP integration | Not required for pilot |
| Email / notifications | Microsoft 365 inbox fetch, categorization, in-app support queue, and outbound proposal replies via Graph (FR-020–FR-025) |

---

## AI Requirements

| Topic | Requirement |
|-------|-------------|
| Provider restrictions | Ollama, OpenAI-compatible APIs, Azure OpenAI; user-selectable at runtime |
| Embeddings | Prefer Ollama embedding models; default `nomic-embed-text`; implementation-flexible; selectable in **Settings**; hybrid degrades gracefully without embeddings |
| Demonstration defaults | Hybrid + Ollama + `qwen3:8b` + `nomic-embed-text`; alternatives acceptable |
| Data classification | Treat customer drawings, offers, calculations, and work instructions as confidential by default |
| Allowed models | User-selectable chat and embedding models per provider; local models preferred for demos |
| Prompt logging | Disabled by default; not persisted unless optional debug mode explicitly enabled and still must exclude document content |
| Cost limits | Hosted providers are optional; user confirmation and local-first defaults reduce unintended cost exposure |
| Audit requirements | Pilot requires disclosure and consent evidence in UX; full audit trail deferred to production |

---

## Monitoring

| Topic | Pilot expectation |
|-------|-------------------|
| Logs | Optional technical debug logging only |
| Metrics | Not required |
| Tracing | Not required |
| Alerts | Not required |

---

## Risk Summary

| Risk | Impact | Mitigation |
|------|--------|------------|
| Real customer data used in live demo with hosted AI | High — GDPR / confidentiality breach | Default local-only processing; explicit hosted-provider consent; synthetic bundled data |
| Hosted AI cost during workshops | Medium | Local model as default; optional hosted providers |
| Weak match quality undermines management demo | High | Bundled realistic sample data; three strategies for comparison; explainable short rationales |
| Missing or inconsistent project folders | Medium | Fixed folder convention; `metadata.json` as primary source; graceful missing-file handling |
| Local model setup friction on demo laptops | Medium | Rule-based mode works out of the box; Ollama supported but not mandatory at first launch |
| Over-scoping toward production ERP quotation | Medium | Requirements limit output to editable draft, clipboard, and PDF export |

---

## Suggested GitHub Issues

| Title | Purpose |
|-------|---------|
| Define `metadata.json` schema and validation rules | Stabilise project catalog and rule-based matching |
| Create bundled `sample-data` projects and synthetic PDFs | Unblock demonstrations without real customer data |
| Specify hosted AI consent UX and disclosure copy | Satisfy FR-015 / NFR-010 with testable acceptance tests |
| Define PDF export layout for inquiry results | Resolved — see FR-020 |
| Spike matching strategies (rule / AI / hybrid) | Compare pilot approaches against bundled scenarios |
| Choose installer approach for self-contained Windows build | Resolved — portable ZIP for pilot; optional MSI later (NFR-003) |

---

## Open Questions

| Question | Owner | Status | Due |
|----------|-------|--------|-----|
| What installer format should be used (MSI, EXE bootstrapper, MSIX)? | Project team | Resolved — **portable ZIP** for pilot; optional **MSI** in future; no MSIX (NFR-003) | — |
| Should the application UI be German, English, or bilingual? | Product owner | Resolved — bilingual; default **German**; selectable in **Settings** (FR-019) | — |
| Should Ollama be auto-detected only, or should the installer optionally install/configure it? | Project team | Resolved — **auto-detect only**; guidance in **Settings**; installer does not install Ollama (FR-014) | — |
| Which local embedding model should be assumed for offline semantic search? | Architecture | Resolved — prefer Ollama embeddings (`nomic-embed-text` default); flexible implementation; hybrid fallback without embeddings (FR-007, FR-014) | — |
| What exact sections must appear in exported PDF results? | Product owner | Resolved — see FR-020 | — |
| Should multiple project root paths be supported simultaneously or one active root only? | Product owner | Resolved — **single active root**; `sample-data/` default when unset (FR-003) | — |
| Are there branding requirements for demo builds (name, logo, company-neutral styling)? | Product owner | Resolved — see NFR-011 | — |
| What is the fourth primary tab besides Inquiry, Results, and Proposal Workspace? | Product owner | Resolved — **Settings** | — |
| Should Proposal Workspace sections be stacked on one screen or split into sub-tabs/accordions? | Product owner | Resolved — single scrollable page with collapsible sections/cards | — |

---

## Completion Checklist

| Criterion | Status |
|-----------|--------|
| Business goals understood | Done |
| Acceptance criteria defined for functional requirements | Done |
| Compliance requirements documented | Done |
| Deployment model known | Done |
| Authentication strategy known | Done |
| Operational responsibilities documented | Done |
| Open questions listed with owners | Done |
| All open questions resolved | Done — requirements approved 2026-06-21 |

---

## Document History

| Version | Date | Change |
|---------|------|--------|
| 0.1 | 2026-06-21 | Initial requirements from discovery workshop |
| 0.2 | 2026-06-21 | Added Proposal Workspace workflow and FR-017 |
| 0.3 | 2026-06-21 | Confirmed four-tab navigation, Proposal Workspace layout, default match behaviour; added FR-018 Settings |
| 0.4 | 2026-06-21 | Renamed **Similar Projects** tab to **Results**; page heading **Top 3 Similar Projects** |
| 0.5 | 2026-06-21 | Added bilingual UI requirement (FR-019); German default |
| 0.6 | 2026-06-21 | Defined PDF export layout and header (FR-020) |
| 0.7 | 2026-06-21 | Single active project root; bundled `sample-data/` default (FR-003) |
| 0.8 | 2026-06-21 | Company-neutral branding and naming (NFR-011) |
| 0.9 | 2026-06-21 | Ollama auto-detect, default models, disabled-strategy UX (FR-007, FR-014) |
| 0.10 | 2026-06-21 | AI Provider Status card in **Settings** (FR-018) |
| 0.11 | 2026-06-21 | Local embeddings preference, Settings selection, hybrid fallback (FR-007, FR-014) |
| 0.12 | 2026-06-21 | Preferred demonstration path documented (Hybrid + Ollama + qwen3:8b + nomic-embed-text) |
| 0.13 | 2026-06-21 | Portable ZIP distribution, local SQLite layout (NFR-003, NFR-012); all open questions resolved |
| 1.0 | 2026-06-21 | Requirements approved for implementation |
