. .\ReadExcelFile.ps1

Get-ExcelData "$pwd\test.xlsx" "select * from [Produce$]" | 
    Sort Region | 
    Format-Table -GroupBy Region -AutoSize