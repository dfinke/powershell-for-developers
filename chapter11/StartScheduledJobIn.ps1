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

$JobName = "TestGetServiceJob"
Get-ScheduledJob -Name $JobName -ea 0 | Unregister-ScheduledJob
Start-ScheduledJobIn $JobName 5 {Get-Service}