.\Add-RoslynLibraries

$slnFileName = Resolve-Path "..\chapter05\BeaverMusic\BeaverMusic.sln"

ForEach ($Project in ([Roslyn.Services.Solution]::Load($slnFileName)).Projects) {

    ForEach($Document in $Project.DocumentIds) {        
        [PSCustomObject] @{
            Filename    = Split-Path $Document.UniqueName -leaf
            ProjectName = $Project.Name
        }
    }
}