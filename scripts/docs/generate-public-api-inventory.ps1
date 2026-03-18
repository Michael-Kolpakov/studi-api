param(
    [string]$OutputPath = "docs/public-api.md"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$files = Get-ChildItem -Recurse -File -Include *.cs Teachio.BLL,Teachio.DAL,Teachio.WebApi |
    Where-Object { $_.FullName -notmatch "\\(bin|obj)\\" }

$pattern = "^[\t ]*public[\t ]+(class|interface|record|enum|struct|delegate)\b|^[\t ]*public[\t ]+.*\("

$results = foreach ($file in $files) {
    Select-String -Path $file.FullName -Pattern $pattern | ForEach-Object {
        [PSCustomObject]@{
            Path = (Resolve-Path -Relative $_.Path).TrimStart('.\\')
            Line = $_.LineNumber
            Text = $_.Line.Trim()
        }
    }
}

$lines = $results |
    Sort-Object Path, Line |
    ForEach-Object { "{0}:{1}: {2}" -f $_.Path, $_.Line, $_.Text }

@(
    "# Public API Inventory"
    ""
    "Generated: $(Get-Date -Format 'yyyy-MM-dd')"
    ""
    "This file documents all current public interfaces (classes, interfaces, records, enums, structs, delegates, and public methods) in production projects: Teachio.BLL, Teachio.DAL, Teachio.WebApi."
    ""
    "## Full List"
    ""
    "```text"
) + $lines + @(
    "```"
    ""
    "## Maintenance Rules"
    ""
    "- Any new public API must be added to this list in the same pull request."
    "- API changes (rename, delete, signature change) must update this file and mention migration impact in PR description."
    "- Prefer XML comments (///) on all public types and methods; keep summaries short and action-oriented."
    "- Breaking changes must include an entry in release notes."
) | Set-Content $OutputPath

Write-Output "Generated $OutputPath"
