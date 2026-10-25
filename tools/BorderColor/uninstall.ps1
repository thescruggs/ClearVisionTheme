# Stops BorderColor, removes it from startup, and restores default window borders.
Set-Location $PSScriptRoot
Get-Process BorderColor -ErrorAction SilentlyContinue | Stop-Process -Force
Remove-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run' -Name 'ClearVisionBorderColor' -ErrorAction SilentlyContinue
if (Test-Path .\BorderColor.exe) { Start-Process -FilePath .\BorderColor.exe -ArgumentList '--reset' -Wait }
Write-Host "BorderColor stopped, removed from startup, borders reset to Windows default."
