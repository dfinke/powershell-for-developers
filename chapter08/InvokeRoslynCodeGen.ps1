# find roslyn libs
$roslynPath = $env:ProgramFiles
$roslyn = "Reference Assemblies\Microsoft\Roslyn\v1.0\Roslyn.Compilers.CSharp.dll"

$dll = Join-Path $roslynPath $roslyn

if(-not (Test-Path $dll)) {
    $roslynPath = ${env:ProgramFiles(x86)}
}

$roslynCompilerDLL = join-path $roslynPath "Reference Assemblies\Microsoft\Roslyn\v1.0\Roslyn.Compilers.dll"
$roslynCSHarpDLL   = join-path $roslynPath "Reference Assemblies\Microsoft\Roslyn\v1.0\Roslyn.Compilers.CSharp.dll"

function Get-RoslynVisitMethods {

    Add-Type -Path $roslynCSHarpDLL

    [Roslyn.Compilers.CSharp.SyntaxVisitor].GetMethods("NonPublic,Instance") |
        Where {$_.name -like 'Visit*'}
}

function Get-RoslynVisitMethodNames { Get-RoslynVisitMethods | Select name }

$code = @"
using System;
using System.Management.Automation;
using Roslyn.Compilers.CSharp;

public class PSRoslynWalker : SyntaxWalker
{
    PSObject module;
    public PSRoslynWalker(PSObject module) {
        this.module = module;
    }

$(
Get-RoslynVisitMethods |
    ForEach {
    @"
   protected override void $($_.name) ($($_.GetParameters()[0].ParameterType.Name) node)
    {
        var method = module.Methods["$($_.name)"];
        if (method != null)
        {
            method.Invoke(node);
        }

        base.$($_.name)(node);
    }

"@
    }
)
}
"@

$r = $roslynCompilerDLL, $roslynCSHarpDLL
Add-Type -TypeDefinition $Code -ReferencedAssemblies $r -IgnoreWarnings