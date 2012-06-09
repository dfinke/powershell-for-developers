.\Add-RoslynLibraries

$slnFileName = Resolve-Path "..\chapter05\BeaverMusic\BeaverMusic.sln"

$result = ForEach ($Project in ([Roslyn.Services.Solution]::Load($slnFileName)).Projects) {
    ForEach($Document in $Project.DocumentIds) {
        New-Object PSObject -Property @{
            Filename    = Split-Path $Document.UniqueName -leaf
            ProjectName = $Project.Name
        }
    }
}

$result | Format-Table -AutoSize