function Search-Google {
    param(
        [Parameter(ValueFromPipeline=$true)]
        [string]$query
    )
    
    Process {
        Start "https://www.google.com/search?q=$query"
    }
}

function ql {$args}

ql win32_service win_bios powershell | Search-Google