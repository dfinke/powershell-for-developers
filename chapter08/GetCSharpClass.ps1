param (
    $targetDirectory
)

.\FindCSharpClass $targetDirectory (New-Module -AsCustomObject {
  
    $script:results = @()

    #   
    function VisitClassDeclaration ($node) {
        $script:results +=  New-Object PSObject -Property @{
            Name  = $node.Identifier.Value
            Class = $node.Identifier
        }
    }
  
    # Implicit interface
    function GetResults { 
        $script:results 
    }
})