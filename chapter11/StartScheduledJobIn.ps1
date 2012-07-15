function Start-ScheduledJobIn {
    param (
        [string]$Name,
        [int]$SecondsFromNow,
        [ScriptBlock]$ScriptBlock
    )

    $trigger = New-JobTrigger `
        -Once `
        -At (Get-Date).AddSeconds($SecondsFromNow)

    Register-ScheduledJob `
        -Name $Name `
        -ScriptBlock $ScriptBlock `
        -Trigger $trigger
}

cls
ipmo PSScheduledJob -Force
Start-ScheduledJobIn TestGetService 5 {Get-Service}