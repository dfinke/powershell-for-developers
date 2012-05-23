$xl = New-Object -ComObject Excel.Application
$xl.Visible = $true
$xl.Workbooks.Add() | Out-Null