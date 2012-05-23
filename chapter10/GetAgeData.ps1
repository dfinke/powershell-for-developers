. .\ReadExcelFile.ps1

Get-ExcelData "$pwd\test.xlsx" "select * from [Ages$]" | 
    Format-Table -AutoSize
