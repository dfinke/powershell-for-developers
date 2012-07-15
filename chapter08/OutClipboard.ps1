function Out-Clipboard {
    param(
        [Parameter(ValueFromPipeline=$true)]
        [string]$text
    )

    Begin {
        Add-Type -AssemblyName PresentationCore
    }

    Process {
        [System.Windows.Clipboard]::SetText($text)
    }
}