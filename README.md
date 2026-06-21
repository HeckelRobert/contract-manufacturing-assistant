# Quotation Accelerator

Portable Windows desktop prototype for reusing historical manufacturing project knowledge during technical review and quotation preparation.

## Prerequisites

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (developers only — pinned in `global.json`)

## For end users

1. Run the installer build once on a developer machine:

```powershell
.\scripts\publish-installer.ps1
```

2. Share `publish/installer/Quotation Accelerator Setup.msi` with pilot users.

3. End users double-click the installer, follow the wizard, then start **Quotation Accelerator** from the Windows Start menu.

The installed app includes bundled `sample-data/` and creates `quotation-accelerator.db` beside the executable on first run. Uninstall via **Settings → Apps**.

## For developers

```powershell
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
  Inquiry/           Customer inquiry domain
  Matching/          Rule-based and hybrid similarity search
  Infrastructure/    SQLite, file system, dispatcher, module registration
  SharedKernel/      Dispatcher abstractions and shared types
installer/           WiX MSI installer project
sample-data/         Bundled demonstration projects
scripts/             Installer publish script
tests/               Unit and architecture tests
```

## Documentation

- [Requirements](docs/requirements.md)
- [Architecture](docs/architecture.md)
- [Security](docs/security.md)
- [Operations](docs/operations.md)

## Manual installer build

```powershell
dotnet build installer/QuotationAccelerator.Installer.wixproj -c Release
```

The MSI is written to `installer/bin/Release/Quotation Accelerator Setup.msi`.
