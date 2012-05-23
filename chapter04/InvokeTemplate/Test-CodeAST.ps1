Add-Type (.\CodeAst.ps1)

$m = New-Module -AsCustomObject {

    $script:FunctionList = @()

    function VisitFunctionDefinition ($ast) {
        $script:FunctionList += New-Object PSObject -Property @{
            Kind = "Function"
            Name = $ast.Name
            StartLineNumber = $ast.Extent.StartLineNumber
        }
    }

    function GetFunctionList {$script:FunctionList}
}

function Get-Ast
{
    param([string]$script)
    [System.Management.Automation.Language.Parser]::ParseInput(
        $script, 
        [ref]$null, 
        [ref]$null
    )
}

$matcher = new-object CommandMatcher $m

$ast = Get-Ast @'
    function test1 {"Say Hello"}
    1..10 | % {$_}
    function test2 {"Say Goodbye"}
    1..10 | % {$_}
    function test3 {"Another function"}
    #function test3 {"This is a comment"}
'@

$ast.Visit($matcher)
$m.GetFunctionList()