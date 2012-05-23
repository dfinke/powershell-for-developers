cls

Add-Type -Path "c:\Program Files\Reference Assemblies\Microsoft\Roslyn\v1.0\Roslyn.Services.dll"
$slnFileName = Resolve-Path  "..\..\C#\BeaverMusic\BeaverMusic.sln"

[Roslyn.Services.Solution]::Load($slnFileName)
