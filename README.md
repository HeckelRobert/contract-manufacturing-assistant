# Quotation Accelerator

Prototype desktop application for contract manufacturing: **find similar past projects quickly** and reuse drawings, manufacturing steps, and quotation knowledge when preparing a new offer.

**Core question the demo answers:** *Have we manufactured something like this before?*

## What you get in the workshop

| Step | What happens |
|------|----------------|
| Enter a customer inquiry | Material, quantity, surface treatment, short description |
| Analyze | The app searches your historical project catalog |
| Review top 3 matches | Similarity score and plain-language reasons |
| Open documents | Project folder and drawing PDF from a past job |
| Proposal workspace | Suggested manufacturing steps and quotation draft — copy into your process |

Bundled sample data is included so you can see results **without configuring anything** on first launch.

## Quick start (workshop / pilot)

You receive **`Quotation Accelerator Setup.msi`** from your presenter or IT (not from building this repository).

1. Double-click the installer and complete the wizard.
2. Start **Quotation Accelerator** from the Windows Start menu.
3. Follow the short demo below — no SDK, no command line, no repository checkout required.

Uninstall later via **Windows Settings → Apps** if needed.

### 5-minute demo path

1. Tab **Anfrage**: Material *Stainless Steel 1.4301*, Oberfläche *Pulverbeschichtet*, Stückzahl *20*, Beschreibung *Stainless enclosure*.
2. Click **Anfrage analysieren**.
3. Tab **Ergebnisse**: best match **PRJ-2019-0142** — try another match, then **Projektordner öffnen** or **Zeichnung öffnen**.
4. Tab **Angebotsarbeitsplatz**: review the draft → **Angebot in Zwischenablage kopieren**.

Default language is German; English is available under **Einstellungen**.

## Requirements (pilot PC)

- Windows 10 or 11 (64-bit)
- No .NET SDK required — the installer is self-contained

## For developers and presenters

Use this section only if you maintain the prototype or **build the MSI** before a workshop.

### Run from source

```powershell
dotnet restore QuotationAccelerator.sln
dotnet build QuotationAccelerator.sln
dotnet run --project src/Desktop/QuotationAccelerator.Desktop.csproj
```

Prerequisites: Windows 10/11, [.NET 10 SDK](https://dotnet.microsoft.com/download) (version pinned in `global.json`).

### Build the installer for distribution

```powershell
.\scripts\publish-installer.ps1
```

Output: `publish/installer/Quotation Accelerator Setup.msi` — share this file with workshop attendees.

Demo PDFs for the flagship sample project are generated automatically during that script. To regenerate them only:

```powershell
.\scripts\generate-sample-pdfs.ps1
```

### Manual installer build

```powershell
dotnet build installer/QuotationAccelerator.Installer.wixproj -c Release
```

MSI path: `installer/bin/Release/Quotation Accelerator Setup.msi`

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
scripts/             Installer and sample-data scripts
tests/               Unit and architecture tests
```

## Documentation

- [Requirements](docs/requirements.md)
- [Architecture](docs/architecture.md)
- [Security](docs/security.md)
- [Operations](docs/operations.md)
