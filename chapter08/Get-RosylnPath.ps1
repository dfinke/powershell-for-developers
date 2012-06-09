
$roslyn = "Reference Assemblies\Microsoft\Roslyn\v1.1\Roslyn.Services.dll"
$dll = Join-Path $env:ProgramFiles $roslyn

if(-not (Test-Path $dll)) {
    $dll = Join-Path ${env:ProgramFiles(x86)} $roslyn    
}

$dll
