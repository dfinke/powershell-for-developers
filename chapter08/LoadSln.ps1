cls

Add-Type -Path "c:\Program Files\Reference Assemblies\Microsoft\Roslyn\v1.0\Roslyn.Services.dll"

$slnFileName = "..\..\C#\BeaverMusic\BeaverMusic.sln"

$result = ForEach ($Project in ([Roslyn.Services.Solution]::Load($slnFileName)).Projects) {
    ForEach($Document in $Project.DocumentIds) {
        New-Object PSObject -Property @{
            Filename    = Split-Path $Document.FileName -leaf
            ProjectName = $Project.DisplayName
        }
    }
}

$result | Format-Table -AutoSize