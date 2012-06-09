.\Add-RoslynLibraries

$slnFileName = Resolve-Path  "..\chapter05\BeaverMusic\BeaverMusic.sln"

[Roslyn.Services.Solution]::Load($slnFileName)