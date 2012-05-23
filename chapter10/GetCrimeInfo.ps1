. .\ReadExcelFile.ps1

$Crimes = Get-ExcelData "$pwd\test.xlsx" "select * from [Crimes$]" 

"Crimes Grouped by Offense"
$Crimes |
    Group Offense -NoElement |
    Sort Count -Descending | Out-String

"Top 10 Days Crimes were committed"  
$Crimes |
    Group Day -NoElement |
    Sort Count -Descending | 
    Select -First 10
    Out-String