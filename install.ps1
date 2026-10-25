# ClearVision on Windows: compile + start the BorderColor tool (registered at sign-in) and apply a theme.
# Works under Windows PowerShell 5.1 and pwsh 7. Safe to re-run.
#   .\install.ps1                       BorderColor + ClearVisionDark
#   .\install.ps1 -Theme HighContrast   BorderColor + ClearVisionTheme (high contrast)
#   .\install.ps1 -Theme None           BorderColor only
param(
    [ValidateSet('Dark', 'HighContrast', 'None')][string]$Theme = 'Dark',
    [switch]$NoBorder,
    [string]$BorderActive = '8C8C8C',
    [string]$BorderInactive = '5A5A5A'
)
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

if (-not $NoBorder) {
    Write-Host 'BorderColor'
    & (Join-Path $PSScriptRoot 'tools\BorderColor\install.ps1') -Active $BorderActive -Inactive $BorderInactive
}

if ($Theme -ne 'None') {
    $file = if ($Theme -eq 'Dark') { 'ClearVisionDark.theme' } else { 'ClearVisionTheme.theme' }
    $name = [System.IO.Path]::GetFileNameWithoutExtension($file)
    $current = (Get-ItemProperty 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Themes' -ErrorAction SilentlyContinue).CurrentTheme
    if ($current -and ((Split-Path $current -Leaf) -like "$name*")) {
        Write-Host "Theme: $name already applied"
    } else {
        Write-Host "Theme: applying $name"
        Start-Process -FilePath (Join-Path $PSScriptRoot $file)   # Windows copies it to %LocalAppData%\Microsoft\Windows\Themes and applies it
        Start-Sleep -Seconds 6
        Get-Process SystemSettings -ErrorAction SilentlyContinue | Stop-Process -Force   # close the Settings page the apply opens
    }
}
Write-Host 'Done.'
