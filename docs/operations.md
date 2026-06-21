# Operations

Handbook version: **v1.3.1**

## Environments

| Environment | Purpose | Location |
|-------------|---------|----------|
| Development | Local build and test | Developer workstation |
| Demonstration | Workshops and management demos | Extracted portable ZIP on business laptop |
| Production | Not applicable for pilot | — |

There is no cloud hosting, staging URL, or Azure infrastructure for this pilot.

---

## Deployment

### Distribution format

Primary deliverable: portable ZIP archive.

```text
QuotationAccelerator-v{version}-win-x64.zip
├── Quotation Accelerator.exe
├── sample-data/
├── appsettings.json
├── quotation-accelerator.db    (created on first run if missing)
└── README.md
```

### Prerequisites (target machine)

| Prerequisite | Required | Notes |
|--------------|----------|-------|
| Windows 10 or 11 x64 | Yes | |
| WebView2 Runtime | Yes | For embedded PDF preview |
| .NET runtime | Bundled | Self-contained publish |
| Ollama | No | Recommended for Hybrid demo path |
| Network share access | No | Only if project root points to SMB path |

### Deployment steps

1. Download `QuotationAccelerator-v{version}-win-x64.zip` from release assets.
2. Extract to any writable local folder (no admin rights required).
3. Run `Quotation Accelerator.exe`.
4. On first launch, bundled `sample-data/` is used automatically if no project root is configured.
5. Optionally install Ollama and pull recommended models:

   ```bash
   ollama pull qwen3:8b
   ollama pull nomic-embed-text
   ```

6. Configure **Settings** as needed for workshop (language, matching strategy, project root).

### Rollback

1. Close the application.
2. Delete the extracted folder.
3. Extract a previous ZIP version if needed.

No uninstaller is required.

### Future installer (optional)

MSI package may be introduced in a later version for IT-managed deployment. Not required for pilot.

---

## Configuration

| Setting | Description | Secret |
|---------|-------------|--------|
| Active project root | Path to `PRJ-*` project folders | No |
| UI language | `de` / `en` | No |
| Matching strategy | Rule-based / AI-assisted / Hybrid | No |
| Ollama base URL | Default `http://localhost:11434` | No |
| Chat model | e.g. `qwen3:8b` | No |
| Embedding model | e.g. `nomic-embed-text` | No |
| OpenAI-compatible base URL + API key | Hosted provider | Yes (key) |
| Azure OpenAI endpoint + key + deployment | Hosted provider | Yes (key) |
| Debug logging enabled | Technical logs in app folder | No |

Configuration is stored in `quotation-accelerator.db` and defaults in `appsettings.json`.

---

## Monitoring

| Signal | Tool | Alert |
|--------|------|-------|
| Application logs | Optional local debug log files | None (pilot) |
| Metrics | Not collected | — |
| Health probes | N/A (desktop app) | — |
| AI cost | User's hosted provider billing | User-managed |

---

## Backup and Restore

### Backup

| Asset | Procedure |
|-------|-----------|
| Application + settings | Copy entire extracted application folder |
| Historical projects | Backup project root file share separately (out of app scope) |

### Restore

1. Restore application folder or re-extract ZIP.
2. Restore `quotation-accelerator.db` if settings and index should be preserved.
3. Point project root to valid `PRJ-*` directory in **Settings**.

---

## Troubleshooting

| Symptom | Likely cause | Action |
|---------|--------------|--------|
| No projects found | Wrong project root or empty folder | Select path containing `PRJ-*` folders; click **Rescan Projects** |
| AI-assisted / Hybrid disabled | Ollama not running; no API key | Start Ollama; pull models; or configure hosted provider |
| PDF preview blank | WebView2 missing or file locked | Install WebView2; open file externally |
| Network share slow | SMB latency | Wait for rescan to complete; use local copy for demos |
| Hosted AI blocked | Consent not granted | Confirm disclosure in Settings |
| Poor match results | Wrong strategy or sparse metadata | Verify `metadata.json`; try Hybrid with Ollama |

---

## Support Responsibilities

| Area | Owner |
|------|-------|
| Portable ZIP build and release | Project team |
| Demo laptop setup | Presenter / customer IT |
| Ollama installation | End user |
| Real customer data in live demos | Presenter (GDPR responsibility) |
| Historical project folder content | Customer |

---

## Document History

| Version | Date | Change |
|---------|------|--------|
| 0.1 | 2026-06-21 | Initial operations document for portable desktop pilot |
