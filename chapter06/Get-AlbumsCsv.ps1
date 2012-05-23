. .\Get-WebData.ps1

$url = "http://dougfinke.com/PowerShellForDevelopers/albums.csv"
(Get-WebData $url -Raw) | ConvertFrom-Csv