# Creates minimal demonstration PDFs for workshop sample projects.
param(
    [string]$ProjectFolder = "sample-data/PRJ-2019-0142_Stainless-Enclosure"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$targetDir = Join-Path $repoRoot $ProjectFolder

if (-not (Test-Path $targetDir)) {
    throw "Project folder not found: $targetDir"
}

function New-SamplePdf {
    param(
        [string]$Path,
        [string]$Title
    )

    $escapedTitle = $Title -replace '\\', '\\\\' -replace '\(', '\(' -replace '\)', '\)'
    $stream = "BT /F1 16 Tf 72 720 Td ($escapedTitle) Tj ET"
    $length = [System.Text.Encoding]::ASCII.GetByteCount($stream)

    $pdf = @"
%PDF-1.4
1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj
2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj
3 0 obj<</Type/Page/MediaBox[0 0 612 792]/Parent 2 0 R/Contents 4 0 R/Resources<</Font<</F1 5 0 R>>>>>>endobj
4 0 obj<</Length $length>>stream
$stream
endstream
endobj
5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica>>endobj
xref
0 6
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000115 00000 n 
0000000274 00000 n 
0000000400 00000 n 
trailer<</Size 6/Root 1 0 R>>
startxref
459
%%EOF
"@

    [System.IO.File]::WriteAllText($Path, $pdf, [System.Text.Encoding]::ASCII)
}

$documents = @{
    "Drawing.pdf" = "Drawing PRJ-2019-0142 Stainless Enclosure"
    "Offer.pdf" = "Historical Offer PRJ-2019-0142"
    "WorkInstruction.pdf" = "Work Instruction PRJ-2019-0142"
    "Fixture.pdf" = "Bending Fixture PRJ-2019-0142"
}

foreach ($entry in $documents.GetEnumerator()) {
    $filePath = Join-Path $targetDir $entry.Key
    New-SamplePdf -Path $filePath -Title $entry.Value
    Write-Host "Created $filePath"
}
