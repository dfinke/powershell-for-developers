$dll = "C:\Program Files\Reference Assemblies\Microsoft\Roslyn\v1.0\Roslyn.Services.dll"

Add-Type -Path $dll -PassThru |
    Where {$_.IsPublic -And $_.BaseType} | Sort Name

[Roslyn.Services.Solution] | Get-Member -Static