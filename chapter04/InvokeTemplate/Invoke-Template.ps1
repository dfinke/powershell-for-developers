function Invoke-Template {
    param(     
        [string]$Path="$pwd",        
        [ScriptBlock]$ScriptBlock
    )

    function Get-Template {
        param($TemplateFileName)

        $content = [IO.File]::ReadAllText( (Join-Path $Path $TemplateFileName) )
        Invoke-Expression "@`"`r`n$content`r`n`"@"
    }

    & $ScriptBlock
}