# Creates demonstration PDFs with realistic multi-line content for workshop sample projects.
param(
    [string]$ProjectFolder = "sample-data/PRJ-2019-0142_Stainless-Enclosure"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$targetDir = Join-Path $repoRoot $ProjectFolder

if (-not (Test-Path $targetDir)) {
    throw "Project folder not found: $targetDir"
}

function Escape-PdfText {
    param([string]$Text)
    return $Text -replace '\\', '\\\\' -replace '\(', '\(' -replace '\)', '\)'
}

function New-SamplePdf {
    param(
        [string]$Path,
        [string[]]$Lines,
        [int]$FontSize = 12,
        [int]$LineHeight = 16
    )

    $y = 750
    $streamParts = New-Object System.Collections.Generic.List[string]
    $streamParts.Add("BT /F1 $FontSize Tf 72 $y Td")

    foreach ($line in $Lines) {
        $escaped = Escape-PdfText $line
        $streamParts.Add("($escaped) Tj")
        if ($line -ne $Lines[-1]) {
            $streamParts.Add("0 -$LineHeight Td")
        }
    }

    $streamParts.Add("ET")
    $stream = ($streamParts -join "`n")
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
    "Drawing.pdf" = @(
        "TECHNICAL DRAWING  DWG-2019-0142-A",
        "Project PRJ-2019-0142  Stainless Steel Enclosure",
        "Customer: Sample Customer A",
        "",
        "Part: Enclosure with hinged door and cable glands",
        "Material: 1.4301 (AISI 304)  t=2.0 mm",
        "Overall dimensions: 480 x 320 x 120 mm",
        "Surface: Powder coat RAL 9016 (textured)",
        "",
        "Manufacturing notes:",
        "- Laser cut flat pattern, deburr all edges",
        "- Bend 4x 90 deg on press brake (see Fixture.pdf)",
        "- TIG weld corner seams, grind flush",
        "- 4x cable gland holes M20 on bottom panel",
        "- Hinge cut-out on front panel per detail A",
        "",
        "Revision: A  |  Scale 1:5  |  Sheet 1 of 1"
    )
    "Offer.pdf" = @(
        "QUOTATION  PRJ-2019-0142",
        "Stainless Steel Enclosure  qty 25",
        "Material 1.4301, powder coated RAL 9016",
        "Lead time: 4 weeks  |  Validity: 30 days",
        "Unit price and setup costs per attached breakdown"
    )
    "WorkInstruction.pdf" = @(
        "WORK INSTRUCTION  PRJ-2019-0142",
        "Sequence: laser cut -> deburr -> bend -> weld -> paint",
        "Bending: use fixture F-0142, 4 stations",
        "Welding: TIG, back purge on internal corners",
        "QC: dimensional check on door opening 298 +/- 0.5 mm"
    )
    "Fixture.pdf" = @(
        "BENDING FIXTURE  F-0142",
        "For stainless enclosure side and front panels",
        "Press brake setup documented 2019-03-12",
        "Reuse approved for similar 1.4301 enclosures"
    )
}

foreach ($entry in $documents.GetEnumerator()) {
    $filePath = Join-Path $targetDir $entry.Key
    New-SamplePdf -Path $filePath -Lines $entry.Value
    Write-Host "Created $filePath"
}
