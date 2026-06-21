# Quotation Accelerator

Portable Windows desktop prototype for reusing historical manufacturing project knowledge during technical review and quotation preparation.

## Prerequisites

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (pinned in `global.json`)
- [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/web-view2/) (for PDF preview — upcoming)
- [Ollama](https://ollama.com/) (optional, for AI-assisted and hybrid matching)

## Quick start

```bash
dotnet restore QuotationAccelerator.sln
dotnet build QuotationAccelerator.sln
dotnet run --project src/Desktop/QuotationAccelerator.Desktop.csproj
```

On first launch the application:

1. Creates `quotation-accelerator.db` beside the executable
2. Defaults to bundled `sample-data/` if no project root is configured
3. Rescans and indexes demonstration projects

## Solution structure

```text
src/
  Desktop/           WPF shell (four tabs)
  Catalog/           Project discovery and indexing
  Inquiry/           Customer inquiry capture
  Matching/          Similarity strategies (planned)
  Proposal/          Manufacturing steps and quotation draft (planned)
  Documents/         PDF preview and file access (planned)
  Export/            Clipboard and PDF export (planned)
  Settings/          Configuration slice (planned)
  Infrastructure/    SQLite, file system, dispatcher, AI adapters
  SharedKernel/      Dispatcher abstractions and shared types
sample-data/         Bundled demonstration projects
tests/               Unit and architecture tests
```

## Documentation

- [Requirements](docs/requirements.md)
- [Architecture](docs/architecture.md)
- [Security](docs/security.md)
- [Operations](docs/operations.md)

## Publish (portable)

```bash
dotnet publish src/Desktop/QuotationAccelerator.Desktop.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -o ./publish
```

Copy `sample-data/` into the publish folder for demonstrations.
