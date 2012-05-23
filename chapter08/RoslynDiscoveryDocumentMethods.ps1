.\Add-RoslynLibraries

$slnFileName = Resolve-Path "..\..\C#\BeaverMusic\BeaverMusic.sln"

$FirstProject = ([Roslyn.Services.Solution]::Load($slnFileName)).Projects | 
                    Select -First 1

$FirstDocument = $FirstProject.Documents | 
    Select -First 1 

$FirstDocument | 
    Get-Member -MemberType Method | Sort Name