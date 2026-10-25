# Compiles BorderColor.exe with the .NET Framework compiler that ships with Windows. No install, no registry.
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$csc = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
& $csc /nologo /target:winexe /optimize /out:BorderColor.exe /r:System.Windows.Forms.dll BorderColor.cs
Get-Item BorderColor.exe | Select-Object Name, Length, LastWriteTime
