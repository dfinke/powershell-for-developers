.\Add-RoslynLibraries

$slnFileName = Resolve-Path "..\..\C#\BeaverMusic\BeaverMusic.sln"

$FirstProject = ([Roslyn.Services.Solution]::Load($slnFileName)).Projects | 
                    Select -First 1

$FirstDocument = $FirstProject.Documents | 
    Select -First 1 

$cancelToken = New-Object System.Threading.CancellationToken
$Root = $FirstDocument.GetSyntaxTree($cancelToken).Root
$Root.Usings | Select name