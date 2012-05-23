param($Name)

$xl = New-Object -ComObject Excel.Application
$wf   = $xl.WorksheetFunction
$wf | 
    Get-Member -MemberType Method | 
    Sort Name |
    Where {$_.Name -match $Name}

$xl.quit()

# Release the COM objects
$wf, $xl | ForEach { 
    [void][Runtime.Interopservices.Marshal]::ReleaseComObject($_)
}