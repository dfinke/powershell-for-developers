$RoslynLibraries = "Roslyn.Compilers.dll", "Roslyn.Services.dll"

$RoslynLibraries | 
    ForEach {
        $dll = Join-Path "Reference Assemblies\Microsoft\Roslyn\v1.0" $_
        .\Add-RoslynLibraries $dll
    }

function Get-NullCancellationToken { New-Object System.Threading.CancellationToken }

function Get-SLNProject {
    param (
        [Parameter(ValueFromPipeline=$true)]
        [string]
        $solution
    )

    Process {
        if( ! (Test-Path $solution) ) {
	    throw "$solution not found"
        }

        [Roslyn.Services.Solution]::Load($solution).Projects
    }
}

Get-SLNProject ..\..\C#\BeaverMusic\BeaverMusic.sln