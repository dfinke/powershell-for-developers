. .\Invoke-Template.ps1

Invoke-Template "$pwd\etc" {
    Import-Csv -Path .\Uml.txt -Header "Name","Type" -Delimiter ":" |
        ForEach {
            $name = $_.Name
            $type = $_.Type
            Get-Template properties.txt
        }
}