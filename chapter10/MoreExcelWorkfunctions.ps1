$xl = New-Object -ComObject Excel.Application
$wf   = $xl.WorksheetFunction

"Dec2Bin   : {0}" -f $wf.Dec2Bin(2)
"Dec2Hex   : {0}" -f $wf.Dec2Hex(16)
"Dec2Oct   : {0}" -f $wf.Dec2Oct(8)
"Factorial : {0}" -f $wf.Fact(9)

$xl.quit()

# Release the COM objects
$wf, $xl | ForEach { 
    [void][Runtime.Interopservices.Marshal]::ReleaseComObject($_)
}