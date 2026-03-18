param(
    [string]$InventoryPath = "docs/public-api.md",
    [string]$OutputPath = "docs/public-api-reference.md"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$lines = Get-Content $InventoryPath
$apiLines = $lines | Where-Object { $_ -match "^[^`].+\.cs:\d+:\s+public\s+" }

$out = New-Object System.Collections.Generic.List[string]
$out.Add("# Public API Reference") | Out-Null
$out.Add("") | Out-Null
$out.Add("Generated: $(Get-Date -Format 'yyyy-MM-dd')") | Out-Null
$out.Add("") | Out-Null
$out.Add("This document contains all public interfaces (functions, classes, methods) from production projects with concise descriptions.") | Out-Null
$out.Add("") | Out-Null

$currentPath = ""

foreach ($line in $apiLines) {
    if ($line -match "^(?<path>.+?):(?<ln>\d+):\s+(?<sig>.+)$") {
        $path = $Matches.path -replace "\\", "/"
        $ln = $Matches.ln
        $sig = $Matches.sig

        if ($path -ne $currentPath) {
            $currentPath = $path
            $out.Add("## $currentPath") | Out-Null
            $out.Add("") | Out-Null
        }

        $desc = "Public member contract."

        if ($sig -match "^public\s+class\s+(\w+)") {
            $desc = ("Class {0} exposes a public contract for this domain or infrastructure area." -f $Matches[1])
        }
        elseif ($sig -match "^public\s+interface\s+(\w+)") {
            $desc = ("Interface {0} defines a public dependency contract for consumers." -f $Matches[1])
        }
        elseif ($sig -match "^public\s+record\s+(\w+)") {
            $desc = ("Record {0} represents a public data contract shared between layers." -f $Matches[1])
        }
        elseif ($sig -match "^public\s+enum\s+(\w+)") {
            $desc = ("Enum {0} defines the allowed public values for business scenarios." -f $Matches[1])
        }
        elseif ($sig -match "^public\s+struct\s+(\w+)") {
            $desc = ("Struct {0} defines a public value-type contract." -f $Matches[1])
        }
        elseif ($sig -match "^public\s+delegate\s+(\w+)") {
            $desc = ("Delegate {0} defines a public callback signature." -f $Matches[1])
        }
        else {
            $desc = "Public method/member contract: document inputs, output, and expected failures."
        }

        $out.Add(("- Line {0}: `{1}`  " -f $ln, $sig)) | Out-Null
        $out.Add(("  {0}" -f $desc)) | Out-Null
        $out.Add("") | Out-Null
    }
}

$out | Set-Content $OutputPath

Write-Output "Generated $OutputPath"
