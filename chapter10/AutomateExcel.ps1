$xl = New-Object -ComObject Excel.Application

$xl.Visible = $true


$xl.quit()

# Release the COM objects
$xl | ForEach { 
    [void][Runtime.Interopservices.Marshal]::ReleaseComObject($_)
}