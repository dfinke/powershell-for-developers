# http://www.microsoft.com/en-us/download/details.aspx?id=13255 x86
# Run in x86 PowerShell Console

function Get-ExcelData {
    param(
        $Datasource,
        $SqlCommand
    )

    $ConnectionString =
       "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=$Datasource;"

    $ConnectionString += 'Extended Properties="Excel 8.0;"'

    $Connection = New-Object Data.OleDb.OleDbConnection $ConnectionString
    $Command    = New-Object Data.OleDb.OleDbCommand $SqlCommand,$Connection
    $Connection.Open()

    $Adapter = New-Object Data.OleDb.OleDbDataAdapter $Command
    $Dataset = New-Object Data.DataSet
    [void] $Adapter.Fill($Dataset)
    $Connection.Close()

    $Dataset.Tables | Select -Expand Rows
}