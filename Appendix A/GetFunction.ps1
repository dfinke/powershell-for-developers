Function Get-Function ([string]$Pattern, [string]$Path="$pwd") {

    $parser  = [System.Management.Automation.PSParser]
    
    $(ForEach( $file in Get-ChildItem $Path -Recurse -Include *.ps1, *.psm1) {
        
        $content = [IO.File]::ReadAllText($file.FullName)
        $tokens  = $parser::Tokenize($content, [ref] $null)   
        $count   = $tokens.Count             

        for($idx=0; $idx -lt $count; $idx += 1) {
            if($tokens[$idx].Content -eq 'function') {
            
                $targetToken = $tokens[$idx+1]
            
                New-Object PSObject -Property @{
                    FileName     = $file.FullName
                    FunctionName = $targetToken.Content
                    Line         = $targetToken.StartLine
                } | Select FunctionName, FileName, Line
            }
        }
    }) | Where {$_.FunctionName -match $Pattern}    
}

Get-Function -Path $PSHOME\Modules\PSDiagnostics | Out-GridView