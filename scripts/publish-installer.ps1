# Builds a Windows MSI installer for pilot distribution.
param(
    [string]$OutputDirectory = "publish/installer",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repoRoot

try {
    Write-Host "Restoring solution..."
    dotnet restore QuotationAccelerator.sln

    Write-Host "Running tests..."
    dotnet test QuotationAccelerator.sln --no-restore

    Write-Host "Ensuring demonstration PDFs..."
    & (Join-Path $PSScriptRoot "generate-sample-pdfs.ps1")

    Write-Host "Ensuring application icon..."
    & (Join-Path $PSScriptRoot "convert-app-icon.ps1")

    $outputPath = Join-Path $repoRoot $OutputDirectory
    if (Test-Path $outputPath) {
        Remove-Item $outputPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $outputPath | Out-Null

    Write-Host "Building installer..."
    dotnet build "installer/QuotationAccelerator.Installer.wixproj" -c Release

    $msiSource = Join-Path $repoRoot "installer\bin/Release/Quotation Accelerator Setup.msi"
    if (-not (Test-Path $msiSource)) {
        throw "Installer build did not produce '$msiSource'."
    }

    $msiTarget = Join-Path $outputPath "Quotation Accelerator Setup.msi"
    Copy-Item -Path $msiSource -Destination $msiTarget -Force

    Write-Host ""
    Write-Host "Done. Share this installer with end users:"
    Write-Host "  $msiTarget"
    Write-Host ""
    Write-Host "They double-click the installer, complete the wizard, then start"
    Write-Host "'Quotation Accelerator' from the Start menu."
}
finally {
    Pop-Location
}
