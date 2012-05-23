function Out-Clipboard {
    param(
        [Parameter(ValueFromPipeline=$true)]
        [string]$text
    )

    Process {
        [System.Windows.Clipboard]::SetText($text)
    }
}