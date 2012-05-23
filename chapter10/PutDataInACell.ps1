$xl = New-Object -ComObject Excel.Application
$xl.Visible = $true

$workbook = $xl.Workbooks.Add()

$sheet1 = $workbook.Worksheets.Item(1)
$sheet1.Cells.Item("A1") = Get-Date