$xl = New-Object -ComObject Excel.Application
$wf   = $xl.WorksheetFunction

$data   = 1,2,3,4
$array = ((1,2,3),(4,5,6),(7,8,9))            

"Median   : {0}" -f $wf.Median($data)            
"StDev    : {0}" -f $wf.StDev($data)
"Var      : {0}" -f $wf.Var($data)
"Transpose: {0}" -f ($wf.Transpose($array) | Out-String)

$xl.quit()

# Release the COM objects
$wf, $xl | ForEach { 
    [void][Runtime.Interopservices.Marshal]::ReleaseComObject($_)
}