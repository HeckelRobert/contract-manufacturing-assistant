# Converts src/Desktop/Assets/app-icon.png to a multi-size Windows .ico file.
param(
    [string]$PngPath = (Join-Path $PSScriptRoot "..\src\Desktop\Assets\app-icon.png"),
    [string]$IcoPath = (Join-Path $PSScriptRoot "..\src\Desktop\Assets\app-icon.ico")
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

function New-BitmapFromPng {
    param([string]$Path, [int]$Size)
    $source = [System.Drawing.Image]::FromFile($Path)
    $bitmap = New-Object System.Drawing.Bitmap($Size, $Size)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.Clear([System.Drawing.Color]::Transparent)
    $graphics.DrawImage($source, 0, 0, $Size, $Size)
    $graphics.Dispose()
    $source.Dispose()
    return $bitmap
}

function Get-PngBytes {
    param([System.Drawing.Bitmap]$Bitmap)
    $stream = New-Object System.IO.MemoryStream
    $Bitmap.Save($stream, [System.Drawing.Imaging.ImageFormat]::Png)
    return $stream.ToArray()
}

$sizes = @(16, 32, 48, 256)
$entries = @()
$imageChunks = New-Object System.Collections.Generic.List[byte[]]

foreach ($size in $sizes) {
    $bitmap = New-BitmapFromPng -Path $PngPath -Size $size
    $pngBytes = Get-PngBytes -Bitmap $bitmap
    $bitmap.Dispose()

    $offset = 6 + (16 * $sizes.Count)
    for ($i = 0; $i -lt $imageChunks.Count; $i++) {
        $offset += $imageChunks[$i].Length
    }

    $entries += [PSCustomObject]@{
        Width    = if ($size -eq 256) { 0 } else { $size }
        Height   = if ($size -eq 256) { 0 } else { $size }
        PngBytes = $pngBytes
        Offset   = $offset
    }
    $imageChunks.Add($pngBytes) | Out-Null
}

$stream = [System.IO.File]::Open($IcoPath, [System.IO.FileMode]::Create)
$writer = New-Object System.IO.BinaryWriter($stream)

# ICONDIR
$writer.Write([UInt16]0) # reserved
$writer.Write([UInt16]1) # type = icon
$writer.Write([UInt16]$entries.Count)

foreach ($entry in $entries) {
    $writer.Write([byte]$entry.Width)
    $writer.Write([byte]$entry.Height)
    $writer.Write([byte]0) # color count
    $writer.Write([byte]0) # reserved
    $writer.Write([UInt16]1) # planes
    $writer.Write([UInt16]32) # bit count
    $writer.Write([UInt32]$entry.PngBytes.Length)
    $writer.Write([UInt32]$entry.Offset)
}

foreach ($entry in $entries) {
    $writer.Write([byte[]]$entry.PngBytes)
}

$writer.Close()
$stream.Close()
Write-Host "Wrote $IcoPath"
