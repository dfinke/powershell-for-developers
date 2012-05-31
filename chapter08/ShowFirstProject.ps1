cls

.\Add-RoslynLibraries.ps1

$slnFileName = Resolve-Path  "..\chapter05\BeaverMusic\BeaverMusic.sln"

[Roslyn.Services.Solution]::Load($slnFileName).Projects | Select -First 1