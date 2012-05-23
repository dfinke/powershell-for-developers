function prompt {
    $host.ui.rawui.WindowTitle = $(get-location)
    
    if(Test-Path .svn) {
        switch -regex (svn st) {
            "^\?" {$other+=1}
            "^A" {$added+=1}
            "^M" {$modified+=1}
            default {}
        }
    
        $prompt_string = "SVN o:$other a:$added m:$modified >"
    } else {
        $prompt_string = "PS >"
    }
    
    Write-Host ($prompt_string) -nonewline -foregroundcolor yellow
    
    return " "
}