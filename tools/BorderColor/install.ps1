# Starts BorderColor now and registers it to start at sign-in (HKCU Run key). Run this yourself when you want it persistent.
param([string]$Active = '8C8C8C', [string]$Inactive = '5A5A5A')
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
if (-not (Test-Path .\BorderColor.exe)) { & .\build.ps1 }
Get-Process BorderColor -ErrorAction SilentlyContinue | Stop-Process -Force
$exe = Join-Path $PSScriptRoot 'BorderColor.exe'
Set-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run' -Name 'ClearVisionBorderColor' -Value "`"$exe`" $Active $Inactive"
Start-Process -FilePath $exe -ArgumentList @($Active, $Inactive)
Write-Host "BorderColor running (active #$Active, inactive #$Inactive) and registered to start at sign-in."
