. .\Invoke-Template.ps1

Invoke-Template "$pwd\etc" {
@"
LastName  : string
FirstName : string
Age       : int
City      : string
State     : string
Zip       : int
"@ | ConvertFrom-Csv -Header "Name","Type" -Delimiter ":" |
        ForEach {
            $name = $_.Name
            $type = $_.Type
            Get-Template properties.txt
        }
}