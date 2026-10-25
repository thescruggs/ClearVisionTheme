# Packs each ClearVision*.theme into a matching .deskthemepack.
# A .deskthemepack is just a Microsoft Cabinet (CAB) archive containing the .theme file.
# Double-clicking either the .theme or the .deskthemepack applies the theme; no other build step exists.
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
foreach ($theme in Get-ChildItem ClearVision*.theme) {
    $pack = [System.IO.Path]::ChangeExtension($theme.Name, '.deskthemepack')
    if (Test-Path $pack) { Remove-Item $pack }
    makecab.exe /D CompressionType=MSZIP $theme.Name $pack | Out-Null
    Get-Item $pack | Select-Object Name, Length
}
Remove-Item setup.inf, setup.rpt -ErrorAction SilentlyContinue
