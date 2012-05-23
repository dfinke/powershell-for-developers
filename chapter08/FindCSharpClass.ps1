param (
    $targetDirectory,
    $module
)

. .\InvokeRoslynCodeGen.ps1
   
dir $targetDirectory -Recurse *.cs | ForEach {
    
    $source   = [IO.File]::ReadAllText($_.FullName)
    $tree     = [Roslyn.Compilers.CSharp.SyntaxTree]::ParseCompilationUnit($source, "", $null, (New-Object System.Threading.CancellationToken) )
    $obj      = New-Object PSRoslynWalker $module
    $obj.Visit($tree.Root)
}

$module.GetResults()